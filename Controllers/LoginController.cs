using Microsoft.AspNetCore.Mvc;
using eCode.Models;
using System.Text.RegularExpressions;

namespace eCode.Controllers
{
    public class LoginController : Controller
    {
        #region Eventos

        [HttpPost]
        public IActionResult Autenticar()
        {
            string mensagem = ValidarCamposLogin();

            if (string.IsNullOrEmpty(mensagem))
            {
                API api = new API();
                eGenericoCampos? eCampo = api.VerificarExisteUsuario(Email, Senha);

                if (eCampo != null)
                {
                    CriarCookie(api.ObterUsuario(eCampo.Id));
                    
                    return Redirect("~/");
                }
                else
                {
                    TempData["Erro"] = "Por favor, verifique os dados digitados, pois os mesmo são inválidos!";
                    TempData["Email"] = Email;
                    TempData["Senha"] = Senha;

                    return Redirect("~/login");
                }
            }
            else
            {
                TempData["Erro"] = mensagem;
                TempData["Email"] = Email;
                TempData["Senha"] = Senha;

                return Redirect("~/login");
            }
        }

        [HttpPost]
        public IActionResult CadastrarCliente()
        {
            string mensagem = ValidarCampos();

            if (string.IsNullOrEmpty(mensagem))
            {
                API api = new API();
                var cookie = Request.Cookies["Cliente"];

                eCliente entidade = new eCliente()
                {
                    Nome = Nome,
                    CPF = CPF,
                    Perfil = cookie != null ? Perfil : "R",
                    Telefone = Telefone,
                    Email = Email,
                    Senha = Senha,
                    DataHora = DateTime.Now,
                    Apoiador = "N",
                    Visivel = "S"
                };

                int id = api.GravarCliente(entidade);
                if (id > 0)
                {
                    CriarCookie(api.ObterUsuario(id));
                    TempData["Sucess"] = "Dados gravados com sucesso.";

                    return Redirect("~/cadastre-se");
                }
                else
                {
                    TempData["Erro"] = "Ocorreu um erro, por favor entre em contato com o suporte.";
                    TempData["Nome"] = Nome;
                    TempData["CPF"] = CPF;
                    TempData["Telefone"] = Telefone;
                    TempData["Nivel"] = Perfil;
                    TempData["Email"] = Email;
                    TempData["Senha"] = Senha;

                    return Redirect("~/cadastre-se");
                }
            }
            else
            {
                TempData["Erro"] = mensagem;
                TempData["Nome"] = Nome;
                TempData["CPF"] = CPF;
                TempData["Telefone"] = Telefone;
                TempData["Nivel"] = Perfil;
                TempData["Email"] = Email;
                TempData["Senha"] = Senha;

                return Redirect("~/cadastre-se");
            }
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult FormCadastro()
        {
            return View("Cadastrar");
        }

        public IActionResult FormRecuperar()
        {
            return View("Senha");
        }

        [HttpPost]
        public IActionResult RecuperarCredencial()
        {
            if (!string.IsNullOrEmpty(Request.Form["txtEmail"].ToString().Trim()))
            {
                TempData["Sucess"] = "Foi encaminhado um E-mail com a recuperação de senha.";

                return Redirect("~/credenciais");
            }
            else
            {
                TempData["Erro"] = "Por favor, o campo \"E-mail\" é obrigatório.";
                TempData["Email"] = Email;

                return Redirect("~/credenciais");
            }
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Sair()
        {
            if (Request.Cookies.ContainsKey("Cliente") || Request.Cookies.ContainsKey("Perfil") || Request.Cookies.ContainsKey("Apoiador") || Request.Cookies.ContainsKey("Codigo"))
            {
                Response.Cookies.Delete("Apoiador");
                Response.Cookies.Delete("Cliente");
                Response.Cookies.Delete("Codigo");
                Response.Cookies.Delete("Perfil");
            }

            return Redirect("~/login");
        }

        #endregion


        #region Metodo Privados

        private void CriarCookie(eCliente? e)
        {
            var opcoesDoCookie = new CookieOptions
            {
                Expires = DateTime.Now.AddHours(1),
            };

            Response.Cookies.Append("Codigo", e.Id.ToString(), opcoesDoCookie);
            Response.Cookies.Append("Cliente", e.Nome, opcoesDoCookie);
            Response.Cookies.Append("Perfil", e.Perfil, opcoesDoCookie);
            Response.Cookies.Append("Apoiador", e.Apoiador, opcoesDoCookie);
        }

        private string ValidarCampos()
        {
            string mensagem = string.Empty;

            if (string.IsNullOrEmpty(Nome.Trim()))
            {
                mensagem = "Por favor, o campo \"Nome\" é obrigatório.";
            }

            if (string.IsNullOrEmpty(mensagem))
            {
                if (!Nome.Contains(" "))
                {
                    mensagem = "Por favor, informe seu Sobrenome.";
                }
            }

            if (string.IsNullOrEmpty(mensagem))
            {
                if (string.IsNullOrEmpty(CPF.Trim()))
                {
                    mensagem = "Por favor, o campo \"CPF\" é obrigatório.";
                }
                else
                {
                    if (CPF.Length != 14)
                    {
                        mensagem = "Por favor, o campo \"CPF\" é inválido.";
                    }
                }
            }

            if (string.IsNullOrEmpty(mensagem))
            {
                if (!Regex.IsMatch(CPF, @"^\d{3}\.\d{3}\.\d{3}-\d{2}$"))
                {
                    mensagem = "Por favor, o campo \"CPF\" é inválido.";
                }
            }

            if (string.IsNullOrEmpty(mensagem))
            {
                if (string.IsNullOrEmpty(Email.Trim()))
                {
                    mensagem = "Por favor, o campo \"E-mail\" é obrigatório.";
                }
            }

            if (string.IsNullOrEmpty(mensagem))
            {
                if (!Email.ToLower().Contains("@"))
                {
                    mensagem = "Por favor, o \"E-mail\" informado é inválido.";
                }
            }

            if (string.IsNullOrEmpty(mensagem))
            {
                if (string.IsNullOrEmpty(Senha.Trim()))
                {
                    mensagem = "Por favor, o campo \"Senha\" é obrigatório.";
                }

                if (string.IsNullOrEmpty(mensagem))
                {
                    if (Senha.Length < 8)
                    {
                        mensagem = "Por favor, a \"Senha\" dever conter no mínimo 8 caracteres.";
                    }

                    if (string.IsNullOrEmpty(mensagem))
                    {
                        if (Senha.Length > 20)
                        {
                            mensagem = "Por favor, a \"Senha\" dever conter no máximo 20 caracteres.";
                        }
                    }
                }
            }

            return mensagem;
        }

        private string ValidarCamposLogin()
        {
            string mensagem = string.Empty;

            if (string.IsNullOrEmpty(Email.Trim()))
            {
                mensagem = "Por favor, o campo \"E-mail\" é obrigatório.";
            }

            if (string.IsNullOrEmpty(mensagem))
            {
                if (!Email.ToLower().Contains("@"))
                {
                    mensagem = "Por favor, o \"E-mail\" informado é inválido.";
                }
            }

            if (string.IsNullOrEmpty(mensagem))
            {
                if (string.IsNullOrEmpty(Senha.Trim()))
                {
                    mensagem = "Por favor, o campo \"Senha\" é obrigatório.";
                }

                if (string.IsNullOrEmpty(mensagem))
                {
                    if (Senha.Length < 8)
                    {
                        mensagem = "Por favor, a \"Senha\" dever conter no mínimo 8 caracteres.";
                    }
                    
                    if (string.IsNullOrEmpty(mensagem))
                    {
                        if (Senha.Length > 20)
                        {
                            mensagem = "Por favor, a \"Senha\" dever conter no máximo 20 caracteres.";
                        }
                    }
                }
            }

            return mensagem;
        }

        #endregion


        #region Propriedades

        private string Nome { get { return Request.Form["txtNome"].ToString(); } }
        private string CPF { get { return Request.Form["txtCpf"].ToString(); } }
        private string Telefone { get { return Request.Form["txtTelefone"].ToString(); } }
        private string Perfil { get { return Request.Form["ddlNivel"].ToString(); } }
        private string Email { get {  return Request.Form["txtEmail"].ToString(); } }
        private string Senha { get { return Request.Form["txtSenha"].ToString(); } }

        #endregion
    }
}
