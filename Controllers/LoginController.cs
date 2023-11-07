﻿using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using eCode.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

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
            API api = new API();
            eCliente entidade = new eCliente()
            {

            };

            if (entidade.Id > 0)
            {
                api.GravarCliente(entidade);
            }

            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public IActionResult FormCadastro()
        {
            return View("Cadastrar");
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


        private string Email { get {  return Request.Form["txtEmail"].ToString(); } }
        private string Senha { get { return Request.Form["txtSenha"].ToString(); } }
    }
}
