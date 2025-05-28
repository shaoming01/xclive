import {defineConfig} from 'vite'
import vue from '@vitejs/plugin-vue'
import Components from 'unplugin-vue-components/vite'
import {AntDesignVueResolver} from 'unplugin-vue-components/resolvers'
import vueJsx from '@vitejs/plugin-vue-jsx'
import autoprefixer from 'autoprefixer'
import flexbugsFixes from 'postcss-flexbugs-fixes'
import AutoImport from 'unplugin-auto-import/vite'
import {viteMockServe} from "vite-plugin-mock";
import analyzer from "vite-bundle-analyzer";

console.log("Vite config is being loaded...");

// https://vitejs.dev/config/
export default defineConfig({
    base: '/',
    plugins: [
        vue(),
        vueJsx(),
        Components({
            resolvers: [
                AntDesignVueResolver({
                    importStyle: false,
                    resolveIcons: true
                }),
            ]
        }),
        AutoImport({
            include: [
                /\.[tj]sx?$/, // .ts, .tsx, .js, .jsx
                /\.vue$/, /\.vue\?vue/, // .vue
            ],
            imports: [
                'vue',
                'vue-router',
                'pinia'
            ],
            dirs: [
                'src/composables',
                'src/utils',
            ],
            resolvers: [
                AntDesignVueResolver(),
            ],
            vueTemplate: true,
            dts: 'auto-imports.d.ts',
        }),
        viteMockServe({
            mockPath: './mock/',//设置模拟数据的存储文件夹
            logger: true,//是否在控制台显示请求日志
        }),
        analyzer({
            analyzerMode: "static", // 使用静态模式，会生成一个可以直接打开的html文件
            fileName: "report",// 生成Html的名称
        })
    ],

    css: {
        postcss: {
            plugins: [
                autoprefixer({
                    overrideBrowserslist: [
                        'Android 4.1',
                        'iOS 7.1',
                        'Chrome > 31',
                        'ff > 31',
                        'ie >= 8',
                        '> 1%',
                    ],
                    grid: true,
                }),
                flexbugsFixes,
            ]
        }
    },
    resolve: {
        alias: {
            '@': '/src'
        },
        extensions: ['.js', '.ts', '.jsx', '.tsx', '.vue']
    },
    server: {
        open: false,
        host: '0.0.0.0',
        port: 7777,
        proxy: {
            // 代理 /api 到本地 .NET 项目
            '/api': {
                target: 'http://localhost:5020', // .NET 应用的地址
                changeOrigin: true,
            }
        }
    },
    build: {
        chunkSizeWarningLimit: 1500,
    }
})
