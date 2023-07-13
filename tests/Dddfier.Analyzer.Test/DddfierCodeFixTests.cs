using System;
using System.IO;
using System.Threading.Tasks;
using Dddfier.Generators;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Testing;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.Testing.Verifiers;
using Xunit;

namespace Dddfier.Analyzer.Test;

public sealed class DddfierCodeFixTests
{
//     [Fact]
//     public async Task ShowCodeFix()
//     {
//         // Arrange
//         const string test =
//             $$"""
//                  namespace TestSample
//                  {
//                      public class TestClass
//                      {
//                          public int Id { get; set; }
//                      }
//                  }
//                  """;
//
//         var expected = new DiagnosticResult(
//                 DddfierAnalyzer.DiagnosticId,
//                 DiagnosticSeverity.Error)
//             .WithLocation(5, 9);
//
//         const string fixedCode = $$"""
//                  using Dddfier.GeneratedCode;
//                  using Dddfier.GeneratedCode.Attributes;
//                  using System; 
//
//                  namespace TestSample
//                  {
//                      [WithIdOf<int>]
//                      public partial class TestClass
//                      {
//                      }
//                  }
//                  """;
//
//         var analyserTest = new CSharpCodeFixTest<DddfierAnalyzer, DddfierAnalyzerCodeFixProvider, XUnitVerifier>
//         {
//             TestState =
//             {
//                 Sources = { test },
//                 ReferenceAssemblies = new ReferenceAssemblies("net6.0",
//                     new PackageIdentity("Microsoft.NETCore.App.Ref", "6.0.0"),
//                     Path.Combine("ref", "net6.0")),
//             },
//             ExpectedDiagnostics = { expected },
//
//             FixedState =
//             {
//                 Sources = { fixedCode }
//             },
//         };
//         // analyserTest.SolutionTransforms.Add((s, p) =>
//         // {
//         //     return s.WithProjectAnalyzerReferences(p.Id,
//         //         new[]
//         //         {
//         //             new AnalyzerFileReference(
//         //                 @"C:\Users\saeed.ganji\.dev\Shahab.Dddifier\src\Dddfier\bin\Debug\net7.0\Dddfier.dll",
//         //                 s.AnalyzerReferences.First().)
//         //         }
//         //     );
//         // });
//
//         analyserTest.TestState.AdditionalReferences.AddRange(new[]
//         {
//             MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
//             MetadataReference.CreateFromFile(typeof(int).Assembly.Location),
//             MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
//             MetadataReference.CreateFromFile(typeof(AggregateIdGenerator).Assembly.Location),
//             // Add any additional references needed for the test compilation
//         });
//
//         // Assert
//         await analyserTest.RunAsync();
//     }
}
