using Microsoft.AspNetCore.Mvc;

namespace eCode.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Autenticar()
        {
            string mensagem = ValidarCamposLogin();

            if (string.IsNullOrEmpty(mensagem))
            {
                return Redirect("~/");
            }
            else
            {
                TempData["Erro"] = mensagem;
                TempData["Email"] = Email;
                TempData["Senha"] = Senha;

                return Redirect("~/login");
            }
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


        private string Email { get {  return Request.Form["email"].ToString(); } }
        private string Senha { get { return Request.Form["senha"].ToString(); } }
    }
}
