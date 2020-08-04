using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using NuGetPackageExplorer.Types;
using NuGetPe;

namespace PackageExplorerViewModel.Rules
{
    [Export(typeof(IPackageRule))]
    internal class MisnamedNativeBuildFileRule : IPackageRule
    {
        #region IPackageRule Members

        public IEnumerable<PackageIssue> Validate(IPackage package, string packagePath)
        {
            var files =
                package.GetFiles().Where(x =>
                    x.Path.EndsWith(".props", StringComparison.OrdinalIgnoreCase) ||
                    x.Path.EndsWith(".targets", StringComparison.OrdinalIgnoreCase));

            foreach (var file in files)
            {
                var path = file.Path;
                var segments = path.Split('\\');

                var frameworkFolder = segments[^2];
                var filename = segments.Last();
                var filenameWithoutExtension = Path.GetFileNameWithoutExtension(filename);

                if (string.Equals(frameworkFolder, "native", StringComparison.OrdinalIgnoreCase) &&
                    filenameWithoutExtension != package.Id)
                {
                    yield return CreatePackageIssueForMisnamedNativeBuildFile(filename, package.Id);
                }
            }
        }

        #endregion


        private static PackageIssue CreatePackageIssueForMisnamedNativeBuildFile(string filename, string packageName)
        {
            return new PackageIssue(
                PackageIssueLevel.Warning,
                "本机构建文件命名错误",
                $"构建文件 '{filename}' 与nuget包名不匹配。对于本机包，这将在被引用时导致不正确的行为。",
                $"重命名构建文件 '{filename}' 以匹配Nuget包名 '{packageName}' 。"
            );
        }
    }
}
