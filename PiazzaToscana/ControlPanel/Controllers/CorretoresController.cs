using PinheiroSereni.Dominio.Contratos;
using PinheiroSereni.Dominio.Control;
using PinheiroSereni.Dominio.Entidades;
using PinheiroSereni.Dominio.Enumeracoes;
using PinheiroSereni.Dominio.Factory;
using PinheiroSereni.Dominio.Security;
using PinheiroSereni.Negocio.Repositories.ControlPanel;
using PinheiroSereni.Negocio.Roles.ControlPanel;
using System;
using System.Web.Mvc;
using System.IO;

namespace ControlPanel.Controllers
{
    public class CorretoresController : SuperController
    {
        #region Browse
        public ActionResult Browse(int? index, string sucesso = "")
        {
            if (sucesso != null)
                if (sucesso.Trim() != "")
                    ViewBag.Sucesso = sucesso;

            return List(index, "40");
        }

        public ActionResult List(int? index, string pageSize)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            ControllerFactory<CorretorCrud> factory = new ControllerFactory<CorretorCrud>();
            IPagedList pagedList = factory.getPagedList(index, int.Parse(pageSize));
            return View(pagedList);
        }
        #endregion

        #region Create
        public ActionResult Create(string sucesso = "")
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            using (PinheiroSereniContext db = new PinheiroSereniContext())
                ViewBag.drpCorretoras = BindDropDownListFactory.Bind<drpCorretoras>(db, "", "Selecione...");
            ViewBag.drpSituacao = BindDropDownListFactory.BindEnum<PinheiroSereni.Dominio.Enumeracoes.drpSituacao>("A");

            if (sucesso != null)
                if (sucesso.Trim() != "")
                    ViewBag.Sucesso = sucesso;

            return View();
        }

        [HttpPost]
        public ActionResult Create(CorretorRepository value)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
               return RedirectToAction("Index","Home");

            value.mensagem = new Validate();

            if (ModelState.IsValid)
                try
                {
                    var fileName = "CorretorNulo.png";

                    // Se for informado o arquivo da foto para upload
                    if (this.Request.Files.Count > 0)
                        if (this.Request.Files [0].FileName.Trim() != "")
                            {
                                #region verifica o tamanho da foto
                                if (this.Request.Files[0].ContentLength < 15360 || this.Request.Files[0].ContentLength > 71680) // entre 15 kb e 70 kb 
                                {
                                    value.mensagem.Code = 100;
                                    value.mensagem.Field = "foto";
                                    value.mensagem.Message = "Tamanho da foto do corretor inválido. Arquivo fora das dimensões permitida.";
                                    value.mensagem.MessageBase = "O tamanho do arquivo da foto deve estar entre 15kb e 70kb";
                                    throw new PinheiroSereniException(value.mensagem); 
                                }
                                #endregion

                                #region Verifica o formato do arquivo
                                System.IO.FileInfo f = new FileInfo(Request.Files[0].FileName);

                                if (f.Extension.ToLower() != ".png")
                                {
                                    value.mensagem.Field = "foto";
                                    value.mensagem.Code = 101;
                                    value.mensagem.Message = "O arquivo deve ser no formato PNG.";
                                    value.mensagem.MessageBase = "A extensão do arquivo da foto deve ser .PNG";
                                    throw new PinheiroSereniException(value.mensagem); 
                                }
                                #endregion

                                #region Enviar a foto do corretor 
                                fileName = String.Format("{0}.png", Guid.NewGuid().ToString());
                                var imagePath = Path.Combine(Server.MapPath(Url.Content("~/Content/themes/base/images/uploads")), fileName);

                                this.Request.Files[0].SaveAs(imagePath);
                                #endregion
                            }

                    #region gravar os dados do corretor
                    value.mensagem = new Validate();
                    value.corretor.nome = value.corretor.nome.ToUpper();
                    value.corretor.setor = value.corretor.setor.ToUpper();
                    value.corretor.telefone = value.corretor.telefone.Replace("(", "").Replace(")", "").Replace("-", "");
                    value.corretor.foto = fileName;
                    value.corretor.email = value.corretor.email.ToLower();

                    ControllerFactory<CorretorCrud> factory = new ControllerFactory<CorretorCrud>();
                    value = (CorretorRepository)factory.Insert(value);
                    if (value.mensagem.Code > 0)
                        throw new PinheiroSereniException(value.mensagem);
                    #endregion

                    return RedirectToAction("Create", new { sucesso = MensagemPadrao.Message(0).ToString() + ". ID do corretor = " + value.corretor.corretorId.ToString() });
                }
                catch (PinheiroSereniException ex)
                {
                    ModelState.AddModelError(value.mensagem.Field, ex.Result.Message);
                    using (PinheiroSereniContext db = new PinheiroSereniContext())
                        ViewBag.drpCorretoras = BindDropDownListFactory.Bind<drpCorretoras>(db, value.corretor.corretoraId.ToString(), "Selecione...");
                    ViewBag.drpSituacao = BindDropDownListFactory.BindEnum<PinheiroSereni.Dominio.Enumeracoes.drpSituacao>(value.corretor.situacao);
                    return View(value);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    using (PinheiroSereniContext db = new PinheiroSereniContext())
                        ViewBag.drpCorretoras = BindDropDownListFactory.Bind<drpCorretoras>(db, value.corretor.corretoraId.ToString(), "Selecione...");
                    ViewBag.drpSituacao = BindDropDownListFactory.BindEnum<PinheiroSereni.Dominio.Enumeracoes.drpSituacao>(value.corretor.situacao);
                    value.mensagem.MessageBase = MensagemPadrao.Message(999).ToString();
                    return View(value);
                }
            else
            {
                value.mensagem.Code = 999;
                value.mensagem.MessageBase = MensagemPadrao.Message(999).ToString();
                using (PinheiroSereniContext db = new PinheiroSereniContext())
                    ViewBag.drpCorretoras = BindDropDownListFactory.Bind<drpCorretoras>(db, value.corretor.corretoraId.ToString(), "Selecione...");
                ViewBag.drpSituacao = BindDropDownListFactory.BindEnum<PinheiroSereni.Dominio.Enumeracoes.drpSituacao>(value.corretor.situacao);
                return View(value);
            }
        }

        #endregion

        #region Edit
        public ActionResult Edit(string id, string sucesso = "")
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            ControllerFactory<CorretorCrud> factory = new ControllerFactory<CorretorCrud>();
            CorretorRepository value = (CorretorRepository)factory.getObject(int.Parse(id));

            using (PinheiroSereniContext db = new PinheiroSereniContext())
                ViewBag.drpCorretoras = BindDropDownListFactory.Bind<drpCorretoras>(db, value.corretor.corretoraId.ToString().Trim(), "Selecione...");
            ViewBag.drpSituacao = BindDropDownListFactory.BindEnum<PinheiroSereni.Dominio.Enumeracoes.drpSituacao>(value.corretor.situacao);

            if (sucesso != null)
                if (sucesso.Trim() != "")
                {
                    value.mensagem = new Validate() { Code = 0, Message = sucesso };
                    ViewBag.Sucesso = sucesso;
                }

            value.corretor.telefone = PinheiroSereni.Library.Funcoes.FormataTelefone(value.corretor.telefone);

            return View(value);
        }

        [HttpPost]
        public ActionResult Edit(string id, CorretorRepository value)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            value.mensagem = new Validate();

            if (ModelState.IsValid)
                try
                {
                    var fileName = value.corretor.foto;

                    ControllerFactory<CorretorCrud> factory = new ControllerFactory<CorretorCrud>();
                    CorretorRepository r = (CorretorRepository)factory.getObject(int.Parse(id));

                    // Se for informado o arquivo da foto para upload
                    if (this.Request.Files.Count > 0)
                        if (this.Request.Files[0].FileName.Trim() != "")
                        {
                            #region verifica o tamanho da foto
                            if (this.Request.Files[0].ContentLength < 15360 || this.Request.Files[0].ContentLength > 71680) // entre 15 kb e 70 kb 
                            {
                                value.mensagem.Code = 100;
                                value.mensagem.Field = "foto";
                                value.mensagem.Message = "Tamanho da foto do corretor inválido. Arquivo fora das dimensões permitida.";
                                value.mensagem.MessageBase = "O tamanho do arquivo da foto deve estar entre 15kb e 70kb";
                                throw new PinheiroSereniException(value.mensagem);
                            }
                            #endregion

                            #region Verifica o formato do arquivo
                            System.IO.FileInfo f = new FileInfo(Request.Files[0].FileName);

                            if (f.Extension.ToLower() != ".png")
                            {
                                value.mensagem.Field = "foto";
                                value.mensagem.Code = 101;
                                value.mensagem.Message = "O arquivo deve ser no formato PNG.";
                                value.mensagem.MessageBase = "A extensão do arquivo da foto deve ser .PNG";
                                throw new PinheiroSereniException(value.mensagem);
                            }
                            #endregion

                            #region Enviar a foto do corretor
                            if (value.corretor.foto.ToLower() == "corretornulo.png")
                                fileName = String.Format("{0}.png", Guid.NewGuid().ToString());
                            else
                                fileName = value.corretor.foto;

                            var imagePath = Path.Combine(Server.MapPath(Url.Content("~/Content/themes/base/images/uploads")), fileName);

                            this.Request.Files[0].SaveAs(imagePath);
                            #endregion
                        }

                    #region gravar os dados do corretor
                    value.corretor.corretorId = int.Parse(id);
                    value.corretor.nome = value.corretor.nome.ToUpper();
                    value.corretor.setor = value.corretor.setor.ToUpper();
                    value.corretor.telefone = value.corretor.telefone.Replace("(", "").Replace(")", "").Replace("-", "");
                    value.corretor.foto = fileName;
                    value.corretor.email = value.corretor.email.ToLower();

                    value = (CorretorRepository)factory.Edit(value);
                    if (value.mensagem.Code > 0)
                        throw new PinheiroSereniException(value.mensagem);
                    #endregion

                    #region Exclui o arquivo de foto, se necessário
                    if (r.corretor.foto != value.corretor.foto && r.corretor.foto != "CorretorNulo.png")
                    {
                        System.IO.FileInfo f = new FileInfo(Path.Combine(Server.MapPath(Url.Content("~/Content/themes/base/images/uploads")), r.corretor.foto));
                        f.Delete();
                    }

                    #endregion

                    return RedirectToAction("Edit", new { id = id, sucesso = MensagemPadrao.Message(0).ToString() });
                }
                catch (PinheiroSereniException ex)
                {
                    ModelState.AddModelError(value.mensagem.Field, ex.Result.Message);
                    using (PinheiroSereniContext db = new PinheiroSereniContext())
                        ViewBag.drpCorretoras = BindDropDownListFactory.Bind<drpCorretoras>(db, value.corretor.corretoraId.ToString(), "Selecione...");
                    ViewBag.drpSituacao = BindDropDownListFactory.BindEnum<PinheiroSereni.Dominio.Enumeracoes.drpSituacao>(value.corretor.situacao);

                    return View(value);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    using (PinheiroSereniContext db = new PinheiroSereniContext())
                        ViewBag.drpCorretoras = BindDropDownListFactory.Bind<drpCorretoras>(db, value.corretor.corretoraId.ToString(), "Selecione...");
                    ViewBag.drpSituacao = BindDropDownListFactory.BindEnum<PinheiroSereni.Dominio.Enumeracoes.drpSituacao>(value.corretor.situacao);
                    value.mensagem.MessageBase = MensagemPadrao.Message(999).ToString();
                    return View(value);
                }
            else
            {
                value.mensagem.Code = 999;
                value.mensagem.MessageBase = MensagemPadrao.Message(999).ToString();
                using (PinheiroSereniContext db = new PinheiroSereniContext())
                    ViewBag.drpCorretoras = BindDropDownListFactory.Bind<drpCorretoras>(db, value.corretor.corretoraId.ToString(), "Selecione...");
                ViewBag.drpSituacao = BindDropDownListFactory.BindEnum<PinheiroSereni.Dominio.Enumeracoes.drpSituacao>(value.corretor.situacao);
                return View(value);
            }
        }

        #endregion

        #region Delete
        public ActionResult Delete(string id)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            ControllerFactory<CorretorCrud> factory = new ControllerFactory<CorretorCrud>();
            CorretorRepository value = (CorretorRepository)factory.getObject(int.Parse(id));

            using (PinheiroSereniContext db = new PinheiroSereniContext())
                ViewBag.drpCorretoras = BindDropDownListFactory.Bind<drpCorretoras>(db, value.corretor.corretoraId.ToString().Trim(), "Selecione...");
            ViewBag.drpSituacao = BindDropDownListFactory.BindEnum<PinheiroSereni.Dominio.Enumeracoes.drpSituacao>(value.corretor.situacao);

            value.corretor.telefone = PinheiroSereni.Library.Funcoes.FormataTelefone(value.corretor.telefone);

            return View(value);
        }

        [HttpPost]
        public ActionResult Delete(string id, CorretorRepository value)
        {
            if (AccessDenied(System.Web.HttpContext.Current.Session.SessionID))
                return RedirectToAction("Index", "Home");

            value = value ?? new CorretorRepository();
            value.mensagem = new Validate();           

            if (ModelState.IsValid)
                try
                {
                    ControllerFactory<CorretorCrud> factory = new ControllerFactory<CorretorCrud>();
                    value = (CorretorRepository)factory.getObject(int.Parse(id));

                    #region Exclui os dados do corretor
                    value = (CorretorRepository)factory.Delete(value);
                    if (value.mensagem.Code > 0)
                        throw new PinheiroSereniException(value.mensagem);
                    #endregion

                    #region Exclui o arquivo de foto, se necessário
                    if (value.corretor.foto != "CorretorNulo.png")
                    {
                        System.IO.FileInfo f = new FileInfo(Path.Combine(Server.MapPath(Url.Content("~/Content/themes/base/images/uploads")), value.corretor.foto));
                        f.Delete();
                    }

                    #endregion

                    return RedirectToAction("Browse", new { sucesso = MensagemPadrao.Message(0).ToString() });
                }
                catch (PinheiroSereniException ex)
                {
                    ModelState.AddModelError(value.mensagem.Field, ex.Result.Message);
                    using (PinheiroSereniContext db = new PinheiroSereniContext())
                        ViewBag.drpCorretoras = BindDropDownListFactory.Bind<drpCorretoras>(db, value.corretor.corretoraId.ToString(), "Selecione...");
                    ViewBag.drpSituacao = BindDropDownListFactory.BindEnum<PinheiroSereni.Dominio.Enumeracoes.drpSituacao>(value.corretor.situacao);

                    return View(value);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    PinheiroSereniException.saveError(ex, GetType().FullName);
                    using (PinheiroSereniContext db = new PinheiroSereniContext())
                        ViewBag.drpCorretoras = BindDropDownListFactory.Bind<drpCorretoras>(db, value.corretor.corretoraId.ToString(), "Selecione...");
                    ViewBag.drpSituacao = BindDropDownListFactory.BindEnum<PinheiroSereni.Dominio.Enumeracoes.drpSituacao>(value.corretor.situacao);
                    value.mensagem.MessageBase = MensagemPadrao.Message(17).ToString();
                    return View(value);
                }
            else
            {
                ControllerFactory<CorretorCrud> factory = new ControllerFactory<CorretorCrud>();
                value = (CorretorRepository)factory.getObject(int.Parse(id));

                value.mensagem.Code = 999;
                value.mensagem.MessageBase = MensagemPadrao.Message(999).ToString();
                using (PinheiroSereniContext db = new PinheiroSereniContext())
                    ViewBag.drpCorretoras = BindDropDownListFactory.Bind<drpCorretoras>(db, value.corretor.corretoraId.ToString(), "Selecione...");
                ViewBag.drpSituacao = BindDropDownListFactory.BindEnum<PinheiroSereni.Dominio.Enumeracoes.drpSituacao>(value.corretor.situacao);
                return View(value);
            }
        }
        #endregion

    }
}
