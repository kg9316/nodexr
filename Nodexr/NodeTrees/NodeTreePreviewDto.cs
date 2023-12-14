﻿namespace Nodexr.NodeTrees;

using System.Text.Json.Nodes;

public class NodeTreePreviewDto
{
    public string Id { get; set; } = null!;

    public string? Title { get; set; } = null!;

    public string? Expression { get; set; } = null!;

    public JsonObject? Nodes { get; set; }

    public string? Description { get; set; }

    public string? SearchText { get; init; }

    public string? ReplacementRegex { get; init; }
}
