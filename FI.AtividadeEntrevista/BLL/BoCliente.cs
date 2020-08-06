using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;

namespace FI.AtividadeEntrevista.BLL
{
    public class BoCliente
    {
        /// <summary>
        /// Inclui um novo cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        public long Incluir(DML.Cliente cliente)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            long Id = cli.Incluir(cliente);

            // Cadastra beneficiários
            DAL.DaoBeneficiario ben = new DAL.DaoBeneficiario();
            cliente.Beneficiarios.ForEach(beneficiario => {
                beneficiario.IdCliente = Id;
                ben.Incluir(beneficiario);
            });

            return Id;
        }

        /// <summary>
        /// Altera um cliente
        /// </summary>
        /// <param name="cliente">Objeto de cliente</param>
        public void Alterar(DML.Cliente cliente)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            cli.Alterar(cliente);

            // Cadastra beneficiários
            DAL.DaoBeneficiario ben = new DAL.DaoBeneficiario();
            cliente.Beneficiarios.ForEach(cliBen => {

                var beneficiario = ben.Consultar(CPF: cliBen.CPF);

                if (beneficiario == null)
                    ben.Incluir(cliBen);
                else
                {
                    cliBen.Id = beneficiario.Id;
                    ben.Alterar(cliBen);
                }
            });

            // Remove beneficiários
            var beneficios = ben.Listar()
                .Where(beneficiario => cliente.Id.Equals(beneficiario.IdCliente)).ToList();
            var beneficiosExcluidos = beneficios
                .Where(beneficiario =>  !cliente.Beneficiarios.Select(b => b.CPF).Contains(beneficiario.CPF)).ToList();

            beneficiosExcluidos.ForEach(beneficiario => {
                ben.Excluir(beneficiario.Id);
            });

        }

        /// <summary>
        /// Consulta o cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public DML.Cliente Consultar(long id)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            var cliente = cli.Consultar(id);
            // Obtém beneficiários
            DAL.DaoBeneficiario ben = new DAL.DaoBeneficiario();
            var Beneficiario = ben.Listar(IdCliente: cliente.Id);
            cliente.Beneficiarios = Beneficiario;
            return cliente;
        }

        /// <summary>
        /// Excluir o cliente pelo id
        /// </summary>
        /// <param name="id">id do cliente</param>
        /// <returns></returns>
        public void Excluir(long id)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            cli.Excluir(id);
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Listar()
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            var clientes =  cli.Listar();
            // Obtém beneficiários
            DAL.DaoBeneficiario ben = new DAL.DaoBeneficiario();
            for (int i = 0; i < clientes.Count(); i++)
            {
                var Beneficiario = ben.Listar(IdCliente: clientes[i].Id);
                clientes[i].Beneficiarios = Beneficiario;
            }
            return clientes;
        }

        /// <summary>
        /// Lista os clientes
        /// </summary>
        public List<DML.Cliente> Pesquisa(int iniciarEm, int quantidade, string campoOrdenacao, bool crescente, out int qtd)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            var clientes = cli.Pesquisa(iniciarEm,  quantidade, campoOrdenacao, crescente, out qtd);
            DAL.DaoBeneficiario ben = new DAL.DaoBeneficiario();
            for (int i = 0; i < clientes.Count(); i++)
            {
                var Beneficiario = ben.Listar(IdCliente: clientes[i].Id);
                clientes[i].Beneficiarios = Beneficiario;
            }
            return clientes;
        }

        /// <summary>
        /// VerificaExistencia
        /// </summary>
        /// <param name="CPF"></param>
        /// <returns></returns>
        public long VerificarExistencia(string CPF)
        {
            DAL.DaoCliente cli = new DAL.DaoCliente();
            return cli.VerificarExistencia(CPF);
        }
    }
}
