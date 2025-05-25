using System.ComponentModel.DataAnnotations.Schema;

namespace Allinone.Domain.DS.Transactions
{
    public class DSType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
