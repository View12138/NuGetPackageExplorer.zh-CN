﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using NuGetPackageExplorer.Types;
using NuGetPe;
using NuGetPe.AssemblyMetadata;
using PackageExplorerViewModel;

namespace PackageExplorer
{
    [PackageContentViewerMetadata(100, ".pdb", SupportsWindows10S = false)]
    internal class PdbFileViewer : IPackageContentViewer
    {
        public object GetView(IPackageContent selectedFile, IReadOnlyList<IPackageContent> peerFiles)
        {
            DiagnosticsClient.TrackEvent("PdbFileViewer");

            AssemblyDebugDataViewModel? data = null;

            // Get the PE file, exe or dll that matches
            var pe = peerFiles.FirstOrDefault(pc => ".dll".Equals(Path.GetExtension(pc.Name), StringComparison.OrdinalIgnoreCase) ||
                                                     ".exe".Equals(Path.GetExtension(pc.Name), StringComparison.OrdinalIgnoreCase) ||
                                                     ".winmd".Equals(Path.GetExtension(pc.Name), StringComparison.OrdinalIgnoreCase) );

#pragma warning disable CA2000 // Dispose objects before losing scope -- ReadDebugData will dispose
            Stream? peStream = null;
            
            if (pe != null) // we have a matching file
            {
                peStream = StreamUtility.MakeSeekable(pe.GetStream(), true);
            }


            // This might throw an exception because we don't know if it's a full PDB or portable
            // Try anyway in case it succeeds as a ppdb
            try
            {
                var stream = StreamUtility.MakeSeekable(selectedFile.GetStream(), true);
                data = new AssemblyDebugDataViewModel(AssemblyMetadataReader.ReadDebugData(peStream, stream));

                // Tab control with two pages
                var tc = new TabControl()
                {
                    Items =
                        {
                            new TabItem
                            {
                                Header = "PDB 信息",
                                Content = new ScrollViewer
                                {
                                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                    Content = new Controls.PdbInfoViewer
                                    {
                                        DataContext = data
                                    }
                                }
                            },
                            new TabItem
                            {
                                Header = "PDB 来源",
                                Content = new ScrollViewer
                                {
                                    HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                                    VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                                    Content = new Controls.PdbSourcesViewer
                                    {
                                        DataContext = data
                                    }
                                }
                            }
                        }
                };

                return tc;
            }
            catch (ArgumentNullException)
            {

            }
#pragma warning restore CA2000 // Dispose objects before losing scope
            return new TextBlock()
            {
                Text = "完整的 PDB 文件需要在 EXE 或 DLL 旁。"
            };

        }
    }
}
