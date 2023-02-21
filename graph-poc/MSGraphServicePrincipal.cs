using Microsoft.Graph;

namespace graph_poc;

public class MSGraphServicePrincipal
{
    public string appId { get; set; }
    public string displayName { get; set; }
    public List<AppRoleAssignment> assignments { get; set; }
}