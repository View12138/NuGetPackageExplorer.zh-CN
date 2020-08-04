using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using NuGet.Frameworks;
using NuGetPackageExplorer.Types;
using NuGetPe;

namespace PackageExplorerViewModel.Rules
{
    [Export(typeof(IPackageRule))]
    internal class InvalidFrameworkFolderRule : IPackageRule
    {
        #region IPackageRule Members

        public IEnumerable<PackageIssue> Validate(IPackage package, string packagePath)
        {
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var file in package.GetFiles())
            {
                var path = file.Path;
                var parts = path.Split(Path.DirectorySeparatorChar);
                if (parts.Length >= 3 && parts[0].Equals("lib", StringComparison.OrdinalIgnoreCase))
                {
                    set.Add(parts[1]);
                }
            }

            return set.Where(IsInvalidFrameworkName).Select(CreatePackageIssue);
        }

        #endregion

        private bool IsInvalidFrameworkName(string name)
        {
            return NuGetFramework.ParseFrameworkName(name, DefaultFrameworkNameProvider.Instance) == NuGetFramework.UnsupportedFramework;
        }

        private static PackageIssue CreatePackageIssue(string target)
        {
            return new PackageIssue(
                PackageIssueLevel.Warning,
                "无效的框架文件夹",
                $"'lib' 下的文件夹 '{target}' 不能识别为有效的框架名称。",
                "将其重命名为有效的框架名称。"
                );
        }
    }
}
