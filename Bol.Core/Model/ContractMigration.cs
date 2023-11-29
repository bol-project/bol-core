namespace Bol.Core.Model;

public class ContractMigration
{
    public byte[] NewScript { get; set; }
    public string NewScriptHash { get; set; }
    public string CurrentScriptHash { get; set; }
    public string Name { get; set; }
    public string Version  { get; set; }
    public string Author { get; set; }
    public string Email { get; set; }
    public string Description { get; set; }
}
