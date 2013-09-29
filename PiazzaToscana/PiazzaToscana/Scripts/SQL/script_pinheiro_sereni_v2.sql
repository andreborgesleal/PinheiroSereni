use pinheiro_sereni
go

begin tran
----------------------------------------------------
-- Passo 1: Criar as tabelas temporárias, sem indice
----------------------------------------------------

CREATE TABLE Prospect1 (
  email NVARCHAR(200) NOT NULL,
  empreendimentoId INTEGER NOT NULL,
  nome NVARCHAR(50) NOT NULL,
  telefone NVARCHAR(11) NULL,
  dt_cadastro DATETIME NULL,
  isFolderDigital VARCHAR(1) NULL DEFAULT ('N')
)
go

CREATE TABLE SMS1 (
  smsId INTEGER NOT NULL,
  empreendimentoId INTEGER NOT NULL,
  corretorId INTEGER NULL,
  telefone NVARCHAR(11) NOT NULL,
  dt_cadastro DATETIME NOT NULL,
  nome NVARCHAR(30) NOT NULL
)
GO

CREATE TABLE Mensagem1 (
  mensagemId INTEGER NOT NULL,
  email NVARCHAR(200) NULL,
  empreendimentoId INTEGER NOT NULL,
  corretorId INTEGER NULL,
  assunto NVARCHAR(100) NOT NULL,
  mensagem NVARCHAR(500) NOT NULL,
  dt_cadastro DATETIME NOT NULL,
  emailDirecao1 NVARCHAR(200) NULL,
  emailDirecao2 NVARCHAR(200) NULL
)
GO

CREATE TABLE Chat1 (
  chatId INTEGER NOT NULL,
  email NVARCHAR(200) NULL,
  empreendimentoId INTEGER NOT NULL,
  corretorId INTEGER NULL,
  sessaoId NVARCHAR(255) NULL,
  dt_inicio DATETIME NOT NULL,
  dt_fim DATETIME NULL
)
GO

CREATE TABLE ChatMessage1 (
  messageId UNIQUEIDENTIFIER NOT NULL,
  chatId INTEGER NULL,
  email NVARCHAR(200) NULL,
  empreendimentoId INTEGER NOT NULL,
  corretorId INTEGER NULL,
  dt_message DATETIME NOT NULL,
  message NVARCHAR(MAX) NOT NULL
)
GO

------------------------------------------------------------------------------
-- Passo 2: Mover o conteúdo das tabelas originais para as tabelas temporárias
------------------------------------------------------------------------------

-- Prospect
insert INTO Prospect1( email, empreendimentoId, nome, telefone, dt_cadastro, isFolderDigital)
select email, 1, nome, telefone, dt_cadastro, isFOlderDigital from Prospect
go

-- SMS 
insert INTO SMS1(smsId, empreendimentoId, corretorId, telefone, dt_cadastro, nome)
select smsId, 1, corretorId, telefone, dt_cadastro, nome from SMS
go

-- Mensagem
insert INTO Mensagem1( mensagemId, email, empreendimentoId, corretorId, assunto, mensagem, dt_cadastro, emailDirecao1, emailDirecao2 )
select mensagemId, email, 1, corretorId, assunto, mensagem, dt_cadastro, emailDirecao1, emailDirecao2 from Mensagem
go

-- Chat
insert INTO Chat1(chatId, email, empreendimentoId, corretorId, sessaoId, dt_inicio, dt_fim)
select chatId, email, 1, corretorId, sessaoId, dt_inicio, dt_fim from Chat
go

-- ChatMessage
insert INTO ChatMessage1 (messageId, chatId, email, empreendimentoId, corretorId, dt_message, message)
select messageId, chatId, email, 1, corretorId, dt_message, message from ChatMessage
go

commit

begin tran

--------------------------------------------------------------------
-- Passo 3: Detonar com as tabelas originais para incluir novamente
--------------------------------------------------------------------
drop table ChatMessage
drop table Chat
drop table Mensagem
drop table SMS
drop table Prospect

--------------------------------------
-- Passo 4: Criar as tabelas novamente
--------------------------------------
CREATE TABLE Empreendimento (
  empreendimentoId INTEGER NOT NULL,
  nomeEmpreendimento NVARCHAR(40) NULL,
  urlFolderDigital NVARCHAR(500) NULL,
  PRIMARY KEY(empreendimentoId)
)

CREATE TABLE Prospect (
  email NVARCHAR(200) NOT NULL,
  empreendimentoId INTEGER NOT NULL,
  nome NVARCHAR(50) NOT NULL,
  telefone NVARCHAR(11) NULL,
  dt_cadastro DATETIME NULL,
  isFolderDigital VARCHAR(1) NULL DEFAULT ('N')
)

CREATE TABLE SMS (
  smsId INTEGER IDENTITY ( 1,1 ) NOT NULL,
  empreendimentoId INTEGER NOT NULL,
  corretorId INTEGER NULL,
  telefone NVARCHAR(11) NOT NULL,
  dt_cadastro DATETIME NOT NULL,
  nome NVARCHAR(30) NOT NULL
)

CREATE TABLE Mensagem (
  mensagemId INTEGER IDENTITY ( 1,1 ) NOT NULL,
  email NVARCHAR(200) NULL,
  empreendimentoId INTEGER NOT NULL,
  corretorId INTEGER NULL,
  assunto NVARCHAR(100) NOT NULL,
  mensagem NVARCHAR(500) NOT NULL,
  dt_cadastro DATETIME NOT NULL,
  emailDirecao1 NVARCHAR(200) NULL,
  emailDirecao2 NVARCHAR(200) NULL
)

CREATE TABLE Chat (
  chatId INTEGER IDENTITY ( 1,1 ) NOT NULL,
  email NVARCHAR(200) NULL,
  empreendimentoId INTEGER NOT NULL,
  corretorId INTEGER NULL,
  sessaoId NVARCHAR(255) NULL,
  dt_inicio DATETIME NOT NULL,
  dt_fim DATETIME NULL
)

CREATE TABLE ChatMessage (
  messageId UNIQUEIDENTIFIER NOT NULL,
  chatId INTEGER NULL,
  email NVARCHAR(200) NULL,
  empreendimentoId INTEGER NOT NULL,
  corretorId INTEGER NULL,
  dt_message DATETIME NOT NULL,
  message NVARCHAR(MAX) NOT NULL
)

-----------------------------------------------------------------------
-- Passo 5: Mover o conteúdo das tabelas temporárias para as originais 
-----------------------------------------------------------------------

insert INTO Empreendimento values( 1, 'Piazza Toscana', NULL)
insert INTO Empreendimento values( 2, 'San Gennaro' , 'http://www.youblisher.com/p/585069-San-Gennaro-Folder-Virtual/')

-- Prospect
insert INTO Prospect( email, empreendimentoId, nome, telefone, dt_cadastro, isFolderDigital)
select email, 1, nome, telefone, dt_cadastro, isFOlderDigital from Prospect1

-- SMS 
SET IDENTITY_INSERT SMS ON

insert INTO SMS(smsId, empreendimentoId, corretorId, telefone, dt_cadastro, nome)
select smsId, 1, corretorId, telefone, dt_cadastro, nome from SMS1

SET IDENTITY_INSERT SMS OFF

-- Mensagem
SET IDENTITY_INSERT Mensagem ON

insert INTO Mensagem( mensagemId, email, empreendimentoId, corretorId, assunto, mensagem, dt_cadastro, emailDirecao1, emailDirecao2 )
select mensagemId, email, 1, corretorId, assunto, mensagem, dt_cadastro, emailDirecao1, emailDirecao2 from Mensagem1

SET IDENTITY_INSERT Mensagem OFF

-- Chat
SET IDENTITY_INSERT Chat ON

insert INTO Chat(chatId, email, empreendimentoId, corretorId, sessaoId, dt_inicio, dt_fim)
select chatId, email, 1, corretorId, sessaoId, dt_inicio, dt_fim from Chat1

SET IDENTITY_INSERT Chat OFF

-- ChatMessage
insert INTO ChatMessage (messageId, chatId, email, empreendimentoId, corretorId, dt_message, message)
select messageId, chatId, email, 1, corretorId, dt_message, message from ChatMessage1


----------------------------
-- Passo 6: Criar os índices
----------------------------

-- Primary Keys
ALTER TABLE Chat
	ADD CONSTRAINT XPKChat PRIMARY KEY  CLUSTERED (chatId ASC)

ALTER TABLE ChatMessage
	ADD CONSTRAINT XPKChatMessage PRIMARY KEY  CLUSTERED (messageId ASC)

ALTER TABLE Mensagem
	ADD CONSTRAINT XPKMensagem PRIMARY KEY  CLUSTERED (mensagemId ASC)

ALTER TABLE Prospect
	ADD CONSTRAINT XPKProspect PRIMARY KEY  CLUSTERED (email ASC, empreendimentoId ASC)

ALTER TABLE Prospect
	ADD CONSTRAINT XUI_Email UNIQUE (email  ASC, empreendimentoId ASC)

ALTER TABLE SMS
	ADD CONSTRAINT XPKSMS PRIMARY KEY  CLUSTERED (smsId ASC)

-- Foreign Keys
ALTER TABLE Chat
	ADD CONSTRAINT R_2 FOREIGN KEY (corretorId) REFERENCES CorretorOnline(corretorId)

ALTER TABLE Chat
	ADD CONSTRAINT R_3 FOREIGN KEY (email, empreendimentoId) REFERENCES Prospect(email, empreendimentoId)

ALTER TABLE Chat 
	ADD  CONSTRAINT R_13 FOREIGN KEY(sessaoId) REFERENCES Sessao(sessaoId)

ALTER TABLE Chat 
	ADD  CONSTRAINT Rel_02 FOREIGN KEY(empreendimentoId) REFERENCES Empreendimento(empreendimentoId)


ALTER TABLE ChatMessage
	ADD CONSTRAINT R_5 FOREIGN KEY (chatId) REFERENCES Chat(chatId)

ALTER TABLE ChatMessage
	ADD CONSTRAINT R_7 FOREIGN KEY (corretorId) REFERENCES CorretorOnline(corretorId)

ALTER TABLE ChatMessage
	ADD CONSTRAINT R_8 FOREIGN KEY (email, empreendimentoId) REFERENCES Prospect(email, empreendimentoId)


ALTER TABLE Mensagem
	ADD CONSTRAINT R_9 FOREIGN KEY (email, empreendimentoId) REFERENCES Prospect(email, empreendimentoId)

ALTER TABLE Mensagem 
	ADD  CONSTRAINT Rel_04 FOREIGN KEY(empreendimentoId) REFERENCES Empreendimento(empreendimentoId)

ALTER TABLE SMS
	ADD CONSTRAINT R_6 FOREIGN KEY (corretorId) REFERENCES CorretorOnline(corretorId)

ALTER TABLE SMS 
	ADD  CONSTRAINT Rel_05 FOREIGN KEY(empreendimentoId) REFERENCES Empreendimento(empreendimentoId)


ALTER TABLE Prospect 
	ADD  CONSTRAINT Rel_08 FOREIGN KEY(empreendimentoId) REFERENCES Empreendimento(empreendimentoId)

----------------------------------------------
-- Passo 7: Detonar com as tabelas temporárias
----------------------------------------------
drop table ChatMessage1
drop table Chat1
drop table Mensagem1
drop table SMS1
drop table Prospect1

----------------------------------------------
-- Passo 8: Atualizar o path do folder digital
----------------------------------------------

select * From Parametro
update Parametro set valor = '/APP_DATA/' where parametroId = 3
select * From Parametro

select * from Parametro

--rollback tran
commit

----------------------------------------------------------------------------------------------------------------------------------------
alter table empreendimento add urlFolderDigital nvarchar(500) null
go
update Empreendimento set urlFolderDigital = 'http://www.youblisher.com/p/585069-San-Gennaro-Folder-Virtual/' where empreendimentoId = 2
go
----------------------------------------------------------------------------------------------------------------------------------------



select * from Prospect order by dt_cadastro desc
select * From Mensagem order by 1 desc
select * from SMS order by 1 desc
select * From chat order by 1 desc
select * From ChatMessage where chatId = 1094

select * from CorretorOnline
update Sessao set dt_desativacao = GETDATE() where dt_desativacao is null
select * from Parametro

