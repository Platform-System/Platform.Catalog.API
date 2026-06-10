using Platform.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Platform.Catalog.API.Infrastructure.Persistence.Models;

[Table("StockAdjustmentOperations")]
public sealed class StockAdjustmentOperationModel : Entity
{
    public string OperationId { get; set; } = string.Empty;
    public string AdjustmentType { get; set; } = string.Empty;
}
