using PagedList;
using S_DDD.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace S_DDD.Controllers
{
    public class HomeController : Controller
    {
        DB_FSTCEntities1 db_con = new DB_FSTCEntities1();
        // GET: Home
        public ActionResult Home()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AddTrabalho()
        {
            return View();
        }


        //------------------------ INSERIR DADOS DE TRABALHO CIENTIFICOS ----------------------------
        [HttpPost]
        public ActionResult AddTrabalho(tb_trabalho_cientifico model, HttpPostedFileBase arquivo)
        {
            tb_trabalho_cientifico tb_infor = new tb_trabalho_cientifico();

            tb_infor.titulo = model.titulo;
            tb_infor.nome = model.nome;
            tb_infor.email = model.email;
            tb_infor.telefone = model.telefone;
            tb_infor.ano_conclusao = model.ano_conclusao;
            tb_infor.tipo_trabalho = model.tipo_trabalho;
            tb_infor.instituicao_ensino = model.instituicao_ensino;
            tb_infor.curso = model.curso;
            tb_infor.resumo = model.resumo;
            tb_infor.palavra_chave = model.palavra_chave;
            tb_infor.area_estudo = model.area_estudo;
            tb_infor.objectivo_trabalho = model.objectivo_trabalho;
            tb_infor.metodologia = model.metodologia;
            tb_infor.tematica_principal = model.tematica_principal;
            tb_infor.possivel_aplicacao = model.possivel_aplicacao;
            tb_infor.departamento = model.departamento;
            tb_infor.fonte_utilizada = model.fonte_utilizada;
            tb_infor.tipo_doc = model.tipo_doc;
            tb_infor.data_atulizacao = DateTime.Now;
            tb_infor.formato_doc = model.formato_doc;

            // Verifica se há um arquivo anexado
           

            db_con.tb_trabalho_cientifico.Add(tb_infor);
            db_con.SaveChanges();

            TempData["MensagemSucesso"] = "Trabalho científico cadastrado com sucesso!";

            return RedirectToAction("AddTrabalho");
        }


        //--------------------------------- LISTAR TRABALHOS POR PAGINAS -------------------------------------------------
        public ActionResult ListTrabalhos(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var trabalhos = db_con.tb_trabalho_cientifico
                .OrderByDescending(t => t.id_trabalho_cientifico) // Ou use .OrderByDescending(t => t.DataCriacao) se existir
                .ThenByDescending(p => p.data_atulizacao)
                .ToPagedList(pageNumber, pageSize);

            return View(trabalhos);
        }


        [HttpGet]
        public ActionResult AddPedidoInformacao() 
        {
            return View();
        }

        //--------------------------------- INSERIR PEDIDOS DE INFORMACAO -------------------------------
        [HttpPost]
        public ActionResult AddPedidoInformacao(tb_pedido_informacao model)
        {
            tb_pedido_informacao tb_inf = new tb_pedido_informacao
            {
                nome = model.nome,
                telefone = model.telefone,
                email = model.email,
                residencia = model.residencia,
                proficao = model.proficao,
                nacionalidade = model.nacionalidade,
                escolaridade = model.escolaridade,
                data_solicitacao = DateTime.Now,
                tema_pesquisa = model.tema_pesquisa,
                finalidade_informacao = model.finalidade_informacao,
                ponto_situacao = "PENDENTE",
                data_atulizacao = DateTime.Now,
                inf_solicitada = model.inf_solicitada, 
                provincia = model.provincia,
            };

            // Upload de arquivo
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                if (file != null && file.ContentLength > 0)
                {
                    if (file.ContentLength > 1048576) // Limite de 1MB
                    {
                        ModelState.AddModelError("formato_doc", "O arquivo não pode ser maior que 1MB.");
                        return View(model);
                    }
                    using (var binaryReader = new System.IO.BinaryReader(file.InputStream))
                    {
                        tb_inf.formato_doc = binaryReader.ReadBytes(file.ContentLength);
                    }
                }
            }

            TempData["SuccessMessage"] = "Os dados foram gravados com sucesso!";

            db_con.tb_pedido_informacao.Add(tb_inf);
            db_con.SaveChanges();
            return RedirectToAction("AddPedidoInformacao"); 
        }

        //---------------------------------- LISTAR PEDIDOS DE INFORMACAO -----------------------------------------------
        public ActionResult ListPedidoInformacao(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            var pedidos = db_con.tb_pedido_informacao
                .OrderBy(p => p.ponto_situacao == "RESPONDIDO") // Falso (0) = Pendentes primeiro, Verdadeiro (1) = Respondidos depois
                .ThenByDescending(p => p.data_solicitacao) // Depois ordena por data
                .ToPagedList(pageNumber, pageSize);

            return View(pedidos); // Verifique se a View corresponde ao modelo correto!
        }


        //---------------------------------- RELATORIO DE PEDIDO DE INFORMACAO ------------------------------------------
        public ActionResult Relatorio_pedido_inf() 
        {
            return View();
        }

        //----------------------------------- RELATORIO DE SUBMICAO DE TRABALHOS CINTIFICOS -----------------------------
        public ActionResult Relatorio_tcc()
        {
            return View();
        }

        //----------------------------------- DETALHES DE PEDIDOE DE INFORMACAO ----------------------------------------
        public ActionResult DetalhePedidos(int id)
        {
            var info = db_con.tb_pedido_informacao.Where(p => p.id_pedido_informacao == id).ToList();
            return View(info);
        }

        //---------------------------------- DETALHES DE SUBMICAO DE TRABALHOS CIENTIFICOS -----------------------------
        public ActionResult DetalheTrabalhoCientifico(int id)
        {
            var info = db_con.tb_trabalho_cientifico.Where(p => p.id_trabalho_cientifico == id).ToList();

            return View(info);
        }

        //---------------------------------------- PESQUISAR TRABALHO --------------------------------------------------
        [HttpGet]
        public ActionResult SearchTrabalhos(string nome, string titulo)
        {
            // Se nenhum título ou nome for passado, retorna todos os trabalhos
            var resultados = db_con.tb_trabalho_cientifico.AsQueryable();

            // Filtra por nome, se fornecido
            if (!string.IsNullOrEmpty(nome))
            {
                resultados = resultados.Where(t => t.nome.Contains(nome));
            }

            // Filtra por título, se fornecido
            if (!string.IsNullOrEmpty(titulo))
            {
                resultados = resultados.Where(t => t.titulo.Contains(titulo));
            }

            // Executa a consulta e retorna os resultados
            return View("ListTrabalhos", resultados.ToList());
        }

        //--------------------------------------- PESQUISAR O PEDIDO DE INFORMACAO ---------------------------------------
        [HttpGet]
        public ActionResult SearchPedido(string nome)
        {
            // Se nenhum título foi passado, retorna todos os trabalhos
            var resultados = string.IsNullOrEmpty(nome)
                ? db_con.tb_pedido_informacao.ToList()
                : db_con.tb_pedido_informacao
                    .Where(t => t.nome.Contains(nome))
                    .ToList();

            return View("ListPedidoInformacao", resultados);
        }

        //---------------------------------------- CONFIGURACOES ----------------------------------------
        public ActionResult Cofiguracao() 
        {
            return View();
        }

        //--------------------- PROCURAR PEDIDO DE INFORMACAO PELO NOME ---------------------------------
        [HttpGet]
        public ActionResult SearchInformacaoPerioda(string nome, DateTime? dataInicio, DateTime? dataFim)
        {
            var resultados = db_con.tb_pedido_informacao.AsQueryable();

            // Filtra pelo nome, se fornecido
            if (!string.IsNullOrEmpty(nome))
            {
                resultados = resultados.Where(t => t.nome.Contains(nome));
            }

            // Filtra pelo intervalo de datas, se fornecido
            if (dataInicio.HasValue)
            {
                resultados = resultados.Where(t => t.data_solicitacao >= dataInicio.Value);
            }
            if (dataFim.HasValue)
            {
                resultados = resultados.Where(t => t.data_solicitacao <= dataFim.Value);
            }

            return View("Relatorio", resultados.ToList());
        }


        //---------------------- EDITAR O PONTO DE SITUACAO ----------------------
        [HttpPost]
        public ActionResult EditarPontoSituacao(int id)
        {
            try
            {
                // Buscar o registro pelo ID
                var pedido = db_con.tb_pedido_informacao.Find(id);

                if (pedido == null)
                {
                    return HttpNotFound();
                }

                // Atualizar apenas o campo ponto_situacao
                pedido.ponto_situacao = "RESPONDIDO";
                pedido.data_atulizacao = DateTime.Now; // Atualiza a data da última modificação

                // Desativar validação automática para evitar erro de campos obrigatórios
                db_con.Configuration.ValidateOnSaveEnabled = false;
                db_con.SaveChanges();
                db_con.Configuration.ValidateOnSaveEnabled = true;

                TempData["SuccessMessage"] = "Ponto de situação atualizado com sucesso!";
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                var errorMessages = new List<string>();

                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        string message = $"Erro na propriedade '{validationError.PropertyName}': {validationError.ErrorMessage}";
                        errorMessages.Add(message);
                    }
                }

                System.Diagnostics.Debug.WriteLine(string.Join("; ", errorMessages));
                TempData["ErrorMessage"] = string.Join("<br/>", errorMessages);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Ocorreu um erro inesperado: " + ex.Message;
            }

            // Buscar os dados atualizados e retornar para a mesma página
            var pedidos = db_con.tb_pedido_informacao.Where(p => p.id_pedido_informacao == id).ToList();

            // Retorna a mesma view com os dados atualizados
            return View("DetalhePedidos", pedidos);
        }


        public ActionResult ListTrabalhos2()
        {
            return View();
        }


        //---------------------------------------- BUSCAR POR TEMAS, PROVINCIA, PROFISSAO ESCOLARIDADE -------------------------------------
        public ActionResult ContarPedidos(DateTime? dataInicio, DateTime? dataFim, string tema, string provincia, string profissao, string escolaridade)
        {
            var query = db_con.tb_pedido_informacao.AsQueryable();

            // Filtros opcionais
            if (dataInicio.HasValue)
                query = query.Where(x => x.data_solicitacao >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(x => x.data_solicitacao <= dataFim.Value);

            if (!string.IsNullOrEmpty(tema))
                query = query.Where(x => x.tema_pesquisa == tema);

            if (!string.IsNullOrEmpty(provincia))
                query = query.Where(x => x.provincia == provincia);

            if (!string.IsNullOrEmpty(profissao))
                query = query.Where(x => x.proficao == profissao);

            if (!string.IsNullOrEmpty(escolaridade))
                query = query.Where(x => x.escolaridade == escolaridade);

            // Calcula os totais
            int totalPendentes = query.Count(x => x.ponto_situacao == "PENDENTE");
            int totalRespondidos = query.Count(x => x.ponto_situacao == "RESPONDIDO");

            // Passa os valores para a ViewBag
            ViewBag.TotalPendentes = totalPendentes;
            ViewBag.TotalRespondidos = totalRespondidos;

            // Retorna a lista filtrada para a View
            var listaPedidos = query.ToList();

            return View("Relatorio_pedido_inf", listaPedidos);
        }

        //------------------------------------- METHODO PARA CARREGAR GRAFICOS ----------------------------
        public ActionResult Graficos()
        {
            var pedidos = db_con.tb_pedido_informacao
                            .Select(p => new { p.tema_pesquisa })
                            .ToList();
            return View(pedidos);
        }

        //---------------------------------------- BUSCAR POR ANA, TIPO DE TRABALHO, AREA DE ESTUDO, METHODOLOGIA -------------------------------------
        public ActionResult ContarTcc(DateTime? dataInicio, DateTime? dataFim, DateTime? ano_conclusao, string tipo_trabalho, string area_estudo, string metodologia, string tematica_principal)
        {
            var query = db_con.tb_trabalho_cientifico.AsQueryable();

            // Filtros opcionais
            if (dataInicio.HasValue)
                query = query.Where(x => x.data_atulizacao >= dataInicio.Value);

            if (dataFim.HasValue)
                query = query.Where(x => x.data_atulizacao <= dataFim.Value);

            if (ano_conclusao.HasValue)
                query = query.Where(x => x.ano_conclusao.HasValue && x.ano_conclusao.Value.Year == ano_conclusao.Value.Year);

            if (!string.IsNullOrEmpty(tipo_trabalho))
                query = query.Where(x => x.tipo_trabalho == tipo_trabalho);

            if (!string.IsNullOrEmpty(area_estudo))
                query = query.Where(x => x.area_estudo == area_estudo);

            if (!string.IsNullOrEmpty(metodologia))
                query = query.Where(x => x.metodologia == metodologia);

            if (!string.IsNullOrEmpty(tematica_principal))
                query = query.Where(x => x.tematica_principal == tematica_principal);


            // Calcula os totais
            int totalTccEncontrados = query.Count();

            // Passa os valores para a ViewBag
            ViewBag.TotalPendentes = totalTccEncontrados;

            // Retorna a lista filtrada para a View
            var listaTcc = query.ToList();

            return View("Relatorio_tcc", listaTcc);
        }
    }
}