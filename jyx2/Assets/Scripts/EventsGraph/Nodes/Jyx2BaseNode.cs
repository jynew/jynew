using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

[NodeWidth(256)]
[CreateNodeMenu("")]
public class Jyx2BaseNode : Node
{
    [Input] public Node prev;
    [Output] public Node next;
}
