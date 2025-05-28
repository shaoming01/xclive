const vueDocgen = require('vue-docgen-api');
const ts = require('typescript');
const fs = require('fs');
const path = require('path');

// 项目根目录
const projectRoot = path.resolve(__dirname + '/..');
const srcDir = path.join(projectRoot, 'src'); // src 目录路径
const componentsDir = path.join(srcDir, 'components'); // 组件目录路径
const outputFilePath = path.join(srcDir, 'utils/vueComDoc.ts'); // 输出 JSON 文件的路径

// 递归获取目录中的所有文件
function getFiles(dir, extensions) {
    let files = [];
    const items = fs.readdirSync(dir);

    items.forEach(item => {
        const fullPath = path.join(dir, item);
        const stat = fs.statSync(fullPath);

        if (stat.isDirectory()) {
            if (!fullPath.includes('node_modules')) { // 排除 node_modules 目录
                files = files.concat(getFiles(fullPath, extensions));
            }
        } else if (stat.isFile() && extensions.some(ext => item.endsWith(ext))) {
            files.push(fullPath);
        }
    });

    return files;
}

// 创建 TypeScript 项目
function createTypeScriptProgram(filePaths) {
    return ts.createProgram(filePaths, {
        target: ts.ScriptTarget.ESNext,
        module: ts.ModuleKind.CommonJS
    });
}

// 解析 JSDoc 注释
function parseJSDocComments(member, sourceFile) {
    const comments = ts.getLeadingCommentRanges(sourceFile.text, member.pos) || [];
    let description = '';
    let see = null;
    let ignore = false;

    comments.forEach(comment => {
        const commentText = sourceFile.text.substring(comment.pos, comment.end);

        // 清理掉注释开始和结束符号
        const cleanCommentText = commentText.replace(/^\/\*\*|\*\/$/g, '').trim();

        // 匹配 @see 标签并提取其内容
        const seeMatch = cleanCommentText.match(/@see\s+\{?([^\}\n]+)\}?/);
        if (seeMatch) {
            see = seeMatch[1].trim();
        }
        if (cleanCommentText.includes('@ignore')) {
            ignore = true;
        }
        // 处理多行注释，提取除 @see 之外的内容
        const descriptionLines = cleanCommentText.split('\n').map(line => {
            // 去掉开头的 `*` 和其他空白符号
            return line.replace(/^\s*\*\s?/, '').trim();
        }).filter(line => line.length > 0 && !line.includes('@see') && !line.includes('@ignore'));  // 过滤掉空行和包含 @see 的行

        if (descriptionLines.length > 0) {
            description = descriptionLines.join(' ');
        }
    });

    return {
        description: description,
        see: see,
        ignore: ignore,
    };
}


// 解析 TypeScript 文件中的接口
function parseTypeScriptInterfaces(program) {
    const checker = program.getTypeChecker();
    const interfaceTypes = {};
    const enumTypes = {};

    program.getSourceFiles().forEach(sourceFile => {
        if (sourceFile.fileName.includes('node_modules')) return;// 排除 node_modules
        ts.forEachChild(sourceFile, node => {
            // 处理枚举
            if (ts.isEnumDeclaration(node)) {
                const enumName = node.name.text;
                enumTypes[enumName] = [];

                node.members.forEach(member => {
                    const memberName = member.name.getText(sourceFile);
                    const memberValue = member.initializer ? member.initializer.text : memberName; // 获取成员值或默认名称
                    enumTypes[enumName].push({name: memberName, value: memberValue});
                });
            } else if (ts.isTypeAliasDeclaration(node)) {// 处理联合类型（如 'development' | 'staging' | 'production'）
                const typeName = node.name.text;

                if (ts.isUnionTypeNode(node.type)) {
                    const values = node.type.types.map(type => {
                        const val = checker.getTypeAtLocation(type).value;
                        return {name: val, value: val};
                    });
                    enumTypes[typeName] = values;
                }
            } else if (ts.isInterfaceDeclaration(node)) {
                const interfaceName = node.name.text;
                interfaceTypes[interfaceName] = [];
                node.members.forEach(member => {
                    if (!ts.isPropertySignature(member)) return;
                    const propName = member.name.getText(sourceFile);
                    const type = checker.typeToString(checker.getTypeAtLocation(member));

                    // 解析 JSDoc 注释
                    const {description, see, ignore} = parseJSDocComments(member, sourceFile);
                    const required = !member.questionToken; // 如果有问号，说明是可选的

                    interfaceTypes[interfaceName].push({
                        name: propName,
                        type,
                        description,
                        required,
                        see, // 将 @see 标签解析后的值存储在 see 字段中
                        ignore,
                    });
                });

            }


        });

    });
    return {interfaceTypes, enumTypes};
}

//Vue属性中Type名称的解析
function calcVueTypeName(type) {
    if (!type.elements || type.elements.length == 0)
        return type.name;
    if (type.name == 'union') {//多类型
        return type.elements.map(el => calcVueTypeName(el)).join(' | ')
    } else if (type.name == 'Array') {//数组
        const arrItem = calcVueTypeName(type.elements[0])
        return arrItem + '[]';
    } else {//泛型
        const arrItem = type.elements.map(el => calcVueTypeName(el)).join(', ')
        return type.name + '<' + arrItem + '>';
    }
}

// 解析 Vue 文件并生成文档
async function parseVueFiles(vueFiles) {
    const allDocs = {};
    for (const filePath of vueFiles) {
        try {
            const doc = await vueDocgen.parse(filePath);
            const relativeFilePath = '/' + path.relative(projectRoot, filePath).replace(/\\/g, '/'); // 转换路径分隔符

            const props = [];
            if (doc.props) {
                Object.entries(doc.props).forEach(([key, value]) => {
                    let see = '';
                    let ignore = false;
                    if (value.tags && value.tags['see'] && value.tags['see'].length > 0) {
                        see = value.tags['see'][0].description;
                        if (see.indexOf('}') > -1)
                            see = see.substring(0, see.indexOf('}'));
                        if (see.indexOf('{') > -1)
                            see = see.substring(see.indexOf('{') + 1, see.length);
                    }
                    if (value.tags && value.tags['ignore']) {
                        ignore = true;
                    }
                    if (value.name == 'dataSource5') {
                        debugger;
                    }
                    //多个类型时type=union
                    let type = calcVueTypeName(value.type);


                    props.push({
                        name: value.name,
                        type: type,
                        tags: value.tags,
                        description: value.description || '',
                        required: value.required,
                        defaultValue: value.defaultValue || null,
                        see: see,
                        ignore: ignore
                    });
                });
            }

            doc.sourceFiles = undefined;
            // Exclude `sourceFiles` and only keep necessary properties
            allDocs[relativeFilePath] = {
                ...doc,
                props: props,
                description: doc.description || '',
                displayName: doc.displayName || ''
            };

            console.log(`Documentation for ${relativeFilePath} generated.`);
        } catch (error) {
            console.error(`Failed to generate documentation for ${filePath}:`, error);
        }
    }


    return allDocs;
}

// 主线程执行的函数
async function generateDocs() {
    const vueFiles = getFiles(componentsDir, ['.vue']);
    const tsFiles = getFiles(srcDir, ['.ts']);

    const program = createTypeScriptProgram(tsFiles);
    const allType = parseTypeScriptInterfaces(program);

    const allDocs = await parseVueFiles(vueFiles);

    // Combine results and write to file
    const outputData = {
        vueComponents: {...allDocs},
        interfaceTypes: allType.interfaceTypes,
        enumTypes: allType.enumTypes
    };

    const varStr = 'export const vueComDoc = ' + JSON.stringify(outputData, null, 4);
    fs.writeFileSync(outputFilePath, varStr);
    console.log(`All documentation saved to ${outputFilePath}`);
}

// 执行文档生成
generateDocs().catch(err => {
    console.error('Error generating documentation:', err);
});
