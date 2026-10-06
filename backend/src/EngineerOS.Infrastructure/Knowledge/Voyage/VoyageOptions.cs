namespace EngineerOS.Infrastructure.Knowledge.Voyage;

public sealed class VoyageOptions
{
    public const string SectionName = "Voyage";

    public string ApiKey { get; set; } = string.Empty;

    public string Model { get; set; } = "voyage-code-4";

    public int Dimensions { get; set; } = 1024;
}