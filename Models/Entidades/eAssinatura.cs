namespace eCode.Models
{
    public class eAssinatura
    {
        public string Ativo { get; set; }
        public DateTime DataHora { get; set; }
        public DateTime DataPagamento { get; set; }
        public DateTime Expirar { get; set; }
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public string TipoPagamento { get; set; }
        public string TipoPlano { get; set; }
        public double ValorPago { get; set; }
    }
}
