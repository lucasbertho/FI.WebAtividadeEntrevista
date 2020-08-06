using FI.AtividadeEntrevista.BLL;
using WebAtividadeEntrevista.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FI.AtividadeEntrevista.DML;

namespace WebAtividadeEntrevista.Controllers
{
    public class BeneficiarioController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Incluir()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Incluir(BeneficiarioModel model)
        {
            BoBeneficiario bo = new BoBeneficiario();

            List<string> erros = new List<string>();

            if (!this.ModelState.IsValid)
            {
                erros.AddRange((from item in ModelState.Values
                                from error in item.Errors
                                select error.ErrorMessage).ToList());
            }
            // Valida CPF
            if (!Validacao.ValidarCPF(model.CPF))
            {
                erros.Add("Digite um CPF válido");
            }
            // Verifica se CPF já existe na base de dados
            else if (bo.VerificarExistencia(model.CPF) != 0)
            {
                erros.Add("CPF já cadastrado");
            }
            else
            {
                model.Id = bo.Incluir(new Beneficiario()
                {
                    CPF = model.CPF,
                    Nome = model.Nome,
                    IdCliente = model.IdCliente
                });
            }

            if (erros.Count() > 0)
            {
                Response.StatusCode = 400;
                return Json(string.Join("</br>", erros.Distinct()));
            }

            return Json("Cadastro efetuado com sucesso");
        }

        [HttpPost]
        public JsonResult Validar(BeneficiarioModel model)
        {
            BoBeneficiario bo = new BoBeneficiario();

            List<string> erros = new List<string>();

            if (!this.ModelState.IsValid)
            {
                erros.AddRange((from item in ModelState.Values
                                from error in item.Errors
                                select error.ErrorMessage).ToList());
            }
            // Valida CPF
            if (!Validacao.ValidarCPF(model.CPF))
            {
                erros.Add("Digite um CPF válido");
            }
            // Verifica se CPF já existe na base de dados
            else if (bo.VerificarExistencia(model.CPF) != 0)
            {
                erros.Add("CPF do Beneficiário já cadastrado");
            }
            
            if (erros.Count() > 0)
            {
                Response.StatusCode = 400;
                return Json(string.Join("</br>", erros.Distinct()));
            }

            var retorno = Json(model);

            return retorno;
        }

        [HttpPost]
        public JsonResult Alterar(BeneficiarioModel model)
        {
            BoBeneficiario bo = new BoBeneficiario();

            List<string> erros = new List<string>();

            if (!this.ModelState.IsValid)
            {
                erros.AddRange((from item in ModelState.Values
                                from error in item.Errors
                                select error.ErrorMessage).ToList());
            }
            // Valida CPF
            if (!Validacao.ValidarCPF(model.CPF))
            {
                erros.Add("Digite um CPF válido");
            }
            // Verifica se CPF já existe na base de dados
            else if (bo.VerificarExistencia(model.CPF) != model.Id)
            {
                erros.Add("CPF já cadastrado para outro beneficiário");
            }
            else
            {
                bo.Alterar(new Beneficiario()
                {
                    Id = model.Id,
                    CPF = model.CPF,    
                    Nome = model.Nome,
                    IdCliente = model.IdCliente
                });
            }

            if (erros.Count() > 0)
            {
                Response.StatusCode = 400;
                return Json(string.Join("</br>", erros.Distinct()));
            }

            return Json("Cadastro alterado com sucesso");
        }

        [HttpGet]
        public ActionResult Alterar(long id)
        {
            BoBeneficiario bo = new BoBeneficiario();
            Beneficiario beneficiario = bo.Consultar(id);
            Models.BeneficiarioModel model = null;

            if (beneficiario != null)
            {
                model = new BeneficiarioModel()
                {
                    Id = beneficiario.Id,
                    CPF = beneficiario.CPF,
                    Nome = beneficiario.Nome,
                    IdCliente = beneficiario.IdCliente
                };


            }

            return View(model);
        }

        [HttpPost]
        public JsonResult BeneficiarioList(long idCliente, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                int qtd = 0;
                string campo = string.Empty;
                string crescente = string.Empty;
                string[] array = jtSorting.Split(' ');

                if (array.Length > 0)
                    campo = array[0];

                if (array.Length > 1)
                    crescente = array[1];

                List<Beneficiario> beneficiarios = new BoBeneficiario().Pesquisa(idCliente, jtStartIndex, jtPageSize, campo, crescente.Equals("ASC", StringComparison.InvariantCultureIgnoreCase), out qtd);

                //Return result to jTable
                return Json(new { Result = "OK", Records = beneficiarios, TotalRecordCount = qtd });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
    }
}