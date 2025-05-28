using System.Text;
using SchemaBuilder.Svc.Core;

namespace SchemaBuilder.Svc.Svc;

public static class FileSvc
{
    private const string BasePath = "./pageSchema/";

    public static R Save(string? fileName, string? schemaJson, string? packageJson)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return R.Faild("fileName不能为空");
        }

        if (!Directory.Exists(BasePath))
        {
            Directory.CreateDirectory(BasePath);
        }

        var name1 = $"{BasePath}{fileName}_projectSchema.json";
        File.WriteAllText(name1, schemaJson, Encoding.UTF8);
        var name2 = $"{BasePath}{fileName}_packages.json";
        File.WriteAllText(name2, packageJson, Encoding.UTF8);
        return R.OK();
    }

    public static R SaveSchema(string? fileName, string? schemaJson)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return R.Faild("fileName不能为空");
        }

        if (!Directory.Exists(BasePath))
        {
            Directory.CreateDirectory(BasePath);
        }

        var name1 = $"{BasePath}{fileName}_projectSchema.json";
        File.WriteAllText(name1, schemaJson, Encoding.UTF8);
        return R.OK();
    }

    public static R SavePackage(string? fileName, string? packageJson)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return R.Faild("fileName不能为空");
        }

        if (!Directory.Exists(BasePath))
        {
            Directory.CreateDirectory(BasePath);
        }

        var name2 = $"{BasePath}{fileName}_packages.json";
        File.WriteAllText(name2, packageJson, Encoding.UTF8);
        return R.OK();
    }

    public static R<string> ReadSchema(string? fileName)
    {
        var name1 = $"{BasePath}{fileName}_projectSchema.json";
        if (!File.Exists(name1))
        {
            return R.Faild<string>("文件不存在");
        }

        return R.OK(File.ReadAllText(name1, Encoding.UTF8));
    }

    public static R<string> ReadPackage(string? fileName)
    {
        var name2 = $"{BasePath}{fileName}_packages.json";
        if (!File.Exists(name2))
        {
            return R.Faild<string>("文件不存在");
        }

        return R.OK(File.ReadAllText(name2, Encoding.UTF8));
    }
}