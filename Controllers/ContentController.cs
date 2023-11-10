using eCode.Models;
using Microsoft.AspNetCore.Mvc;

namespace eCode.Controllers
{
    public class ContentController : Controller
    {

        #region Eventos POST

        [HttpPost]
        public IActionResult CancelarAssinatura()
        {
            API api = new API();

            eGenericoCampos eAssinatura = new eGenericoCampos()
            {
                Campo = "N",
                Id = string.IsNullOrEmpty(Request.Form["hdnId"]) ? 0 : Convert.ToInt32(Request.Form["hdnId"])
            };

            eGenericoCampos eCliente = new eGenericoCampos()
            {
                Campo = "N",
                Id = Request.Cookies["Codigo"] != null ? Convert.ToInt32(Request.Cookies["Codigo"]) : 0
            };

            api.AlterarClienteApoiador(eCliente);
            api.DeletarPlano(eAssinatura);

            return Redirect("~/dashboard");
        }

        [HttpPost]
        public IActionResult RealizarPagamento()
        {
            string mensagem = ValidarCamposPagamento();

            if (string.IsNullOrEmpty(mensagem))
            {
                API api = new API();
                eAssinatura entidade = new eAssinatura();
                eGenericoCampos eCampos = new eGenericoCampos();

                entidade.Ativo = "S";
                entidade.IdCliente = Request.Cookies["Codigo"] != null ? Convert.ToInt32(Request.Cookies["Codigo"]) : 0;
                entidade.TipoPagamento = TipoPagamento.ToUpper();
                entidade.TipoPlano = TipoPlano.ToUpper();
                entidade.DataHora = DateTime.Now;
                entidade.DataPagamento = !string.Equals(TipoPagamento.ToUpper(), "C") ? DateTime.Now.AddDays(3) : DateTime.Now.AddMinutes(5);

                eCampos.Campo = "S";
                eCampos.Id = entidade.IdCliente;

                switch (TipoPlano.ToUpper())
                {
                    case "B":
                        entidade.Expirar = entidade.DataPagamento.AddMonths(3);
                        entidade.ValorPago = 9.90;
                        break;

                    case "M":
                        entidade.Expirar = entidade.DataPagamento.AddMonths(6);
                        entidade.ValorPago = 19.90;
                        break;

                    case "P":
                        entidade.Expirar = entidade.DataPagamento.AddMonths(12);
                        entidade.ValorPago = 27.90;
                        break;
                }

                api.AlterarClienteApoiador(eCampos);
                string? retorno = api.GravarAssinatura(entidade);

                if (!string.IsNullOrEmpty(retorno) && string.Equals(retorno.ToLower(), "sucesso"))
                {
                    CriarCookie("S");
                    TempData["Sucess"] = string.Equals(TipoPagamento.ToUpper(), "B") ? string.Format("{0}, você receberá um boleto por e-mail", Request.Cookies["Cliente"]) : "Compra aprovada com sucesso.";
                    return Redirect("~/dashboard");
                }
                else
                {
                    TempData["Erro"] = "Ocorreu um erro, por favor entre em contato com o suporte.";
                    TempData["CVV"] = CVV;
                    TempData["DataValidade"] = DataValidade;
                    TempData["NumeroCartao"] = NumeroCartao;

                    return Redirect(!string.IsNullOrEmpty(TipoPlano) ? string.Format("~/pagamentos?plan={0}", TipoPlano) : "~/");
                }
            }
            else
            {
                TempData["Erro"] = mensagem;
                TempData["CVV"] = CVV;
                TempData["DataValidade"] = DataValidade;
                TempData["NumeroCartao"] = NumeroCartao;

                return Redirect(!string.IsNullOrEmpty(TipoPlano) ? string.Format("~/pagamentos?plan={0}", TipoPlano) : "~/");
            }
        }

        #endregion


        #region Eventos GET

        public IActionResult CarregarAssinatura()
        {
            return View("Assinatura", new API().ObterAssinatura(Convert.ToInt32(Request.Cookies["Codigo"])));
        }

        public IActionResult CarregarDesafios()
        {
            API api = new API();
            string filtro = string.IsNullOrEmpty(Request.Query["filtro"]) ? string.Empty : Request.Query["filtro"];
            string? apoiador = Request.Cookies["Apoiador"] != null ? Request.Cookies["Apoiador"] : "N";
            string? perfil = Request.Cookies["Perfil"] != null ? Request.Cookies["Perfil"] : "R";

            return View("Desafios", api.ListarDesafios(filtro, apoiador, perfil));
        }

        public IActionResult FormPagamento()
        {
            return View("Pagamentos");
        }

        public IActionResult Index()
        {
            return View();
        }

        #endregion


        #region Metodos Privados

        private void CriarCookie(string apoiador)
        {
            Response.Cookies.Delete("Apoiador");

            var opcoesDoCookie = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(1),
            };

            Response.Cookies.Append("Apoiador", apoiador, opcoesDoCookie);
        }

        private string ValidarCamposPagamento()
        {
            string mensagem = string.Empty;
            
            if (string.IsNullOrEmpty(TipoPlano))
            {
                mensagem = "Por favor, escolha um \"Plano\", para continuamos.";
            }

            if (string.IsNullOrEmpty(mensagem))
            {
                if (string.IsNullOrEmpty(TipoPagamento))
                {
                    mensagem = "Por favor, o campo \"Método de pagamento\" é obrigatório.";
                }
            }

            if (string.IsNullOrEmpty(mensagem))
            {
                if (string.Equals(TipoPagamento.ToUpper(), "C"))
                {
                    if (string.IsNullOrEmpty(NumeroCartao.Trim()))
                    {
                        mensagem = "Por favor, o campo \"Número do cartão\" é obrigatório.";
                    }

                    if (string.IsNullOrEmpty(mensagem))
                    {
                        if (string.IsNullOrEmpty(DataValidade))
                        {
                            mensagem = "Por favor, o campo \"Data de válidade\" é obrigatório.";
                        }
                    }

                    if (string.IsNullOrEmpty(mensagem))
                    {
                        if (string.IsNullOrEmpty(CVV))
                        {
                            mensagem = "Por favor, o campo \"CVV\" é obrigatório.";
                        }
                    }
                }
            }

            return mensagem;
        }

        #endregion


        #region Propriedades

        private string CVV { get { return Request.Form["txtCvv"].ToString(); } }
        private string DataValidade { get { return Request.Form["txtDataValidade"].ToString(); } }
        private string NumeroCartao { get { return Request.Form["txtNumero"].ToString(); } }
        private string TipoPagamento { get { return Request.Form["ddlTipoPagamento"].ToString(); } }
        private string TipoPlano { get { return Request.Form["hdnTipoPlano"].ToString(); } set { } }

        #endregion
    }
}
