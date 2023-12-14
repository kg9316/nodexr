﻿namespace Nodexr.NodeTrees;

using MediatR;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public record CreateAnonymousNodeTreeCommand(
    [property: JsonPropertyName("nodes")] JsonObject Nodes
) : IRequest<string>
{
    public string? SearchText { get; set; }

    public string? ReplacementRegex { get; set; }
}
