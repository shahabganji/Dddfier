using System.Text;
using Microsoft.CodeAnalysis;

namespace Dddfier.Console;

public class MyWalker : CSharpSyntaxWalker
{
    public MyWalker():base(SyntaxWalkerDepth.Token)
    {
    }
    
    public readonly StringBuilder Sb = new();
    public override void VisitToken(SyntaxToken token)
    {
        Sb.Append(token.ToFullString());
    }
}
