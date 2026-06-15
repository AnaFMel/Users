namespace Users.Infra.CrossCutting.Security
{
    public class JwtOptions
    {
        public string Key { get; set; } = null!;
        public string[] Issuers { get; set; } = null!;
        public int Seconds { get; set; }
    }
}
