using System;

[Serializable]
public readonly struct PanelID : IEquatable<PanelID>
{
    public readonly string Value;

    public PanelID(string value)
    {
        Value = value;
    }

    public bool Equals(PanelID other) => Value == other.Value;
    public override bool Equals(object obj) => obj is PanelID other && Equals(other);
    public override int GetHashCode() => Value?.GetHashCode() ?? 0;
    public override string ToString() => Value;

    public static implicit operator string(PanelID id) => id.Value;
    public static explicit operator PanelID(string value) => new (value);
}