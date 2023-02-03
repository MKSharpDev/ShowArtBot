using System;
using System.Collections.Generic;

namespace ShowArtBot.Hermitage;

public partial class ArtObject
{
    public long ArtObjectId { get; set; }

    public string ArtName { get; set; } = null!;

    public string ArtDiscrAuthor { get; set; } = null!;

    public string ArtDiscrFrom { get; set; } = null!;

    public string ImageName { get; set; } = null!;
}
