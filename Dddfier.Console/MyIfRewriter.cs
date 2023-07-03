using Microsoft.CodeAnalysis;

namespace Dddfier.Console;

public sealed class MyIfRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode? VisitIfStatement(IfStatementSyntax node)
    {
        // removes the node
        // return null; 
        
        
        // return the same exact node
        // return node;
        
        // if you don;t call this, it won't visit the children nodes, like the body of the if statement
        // return base.VisitIfStatement(node);
        
        
        // return entirely new SyntaxNode, to replace this node
        var body = node.Statement;
        var block = SyntaxFactory.Block(body);
        var newIfStatement = node.WithStatement(block);
        return newIfStatement;
    }
    
}
