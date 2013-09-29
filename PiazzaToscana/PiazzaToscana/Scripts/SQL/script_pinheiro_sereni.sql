use pinheiro_sereni
go

CREATE TABLE Chat
( 
	chatId               integer IDENTITY ( 1,1 ) ,
	dt_inicio            datetime  NOT NULL ,
	dt_fim               datetime  NULL ,
	corretorId           integer  NULL ,
	sessionId            nvarchar(max)  NULL ,
	email                nvarchar(200)  NULL 
)
go

ALTER TABLE Chat
	ADD CONSTRAINT XPKChat PRIMARY KEY  CLUSTERED (chatId ASC)
go

CREATE TABLE ChatMessage
( 
	messageId            uniqueidentifier  NOT NULL ,
	chatId               integer  NOT NULL ,
	dt_message           datetime  NOT NULL ,
	message              nvarchar(max)  NOT NULL ,
	corretorId           integer  NULL ,
	email                nvarchar(200)  NULL 
)
go

ALTER TABLE ChatMessage
	ADD CONSTRAINT XPKChatMessage PRIMARY KEY  CLUSTERED (messageId ASC)
go

CREATE TABLE Corretora
( 
	corretoraId          integer IDENTITY ( 1,1 ) ,
	nome                 nvarchar(50)  NOT NULL 
)
go

ALTER TABLE Corretora
	ADD CONSTRAINT XPKCorretora PRIMARY KEY  CLUSTERED (corretoraId ASC)
go

CREATE TABLE CorretorOnline
( 
	corretorId           integer IDENTITY ( 1,1 ) ,
	nome                 varchar(20)  NULL ,
	setor                nvarchar(30)  NULL ,
	telefone             nvarchar(11)  NULL ,
	email                nvarchar(200)  NULL ,
	foto                 nvarchar(200)  NULL ,
	corretoraId          integer  NULL ,
	indexEscala          integer  NOT NULL ,
	situacao             nvarchar(1)  NOT NULL 
)
go

ALTER TABLE CorretorOnline
	ADD CONSTRAINT XPKCorretorOnline PRIMARY KEY  CLUSTERED (corretorId ASC)
go

CREATE TABLE Mensagem
( 
	mensagemId           integer IDENTITY ( 1,1 ) ,
	email                nvarchar(200)  NULL ,
	assunto				 nvarchar(100)  NOT NULL ,
	mensagem             nvarchar(500)  NOT NULL ,	
	dt_cadastro          datetime  NOT NULL ,
	emailDirecao1        nvarchar(200)  NULL ,
	emailDirecao2        nvarchar(200)  NULL 
)
go

ALTER TABLE Mensagem
	ADD CONSTRAINT XPKMensagem PRIMARY KEY  CLUSTERED (mensagemId ASC)
go

CREATE TABLE Parametro
( 
	parametroId          integer NOT NULL,
	nome                 nvarchar(60)  NOT NULL ,
	valor                nvarchar(max)  NOT NULL 
)
go

ALTER TABLE Parametro
	ADD CONSTRAINT XPKParametro PRIMARY KEY  CLUSTERED (parametroId ASC)
go

CREATE TABLE Prospect
( 
	nome                 nvarchar(50)  NOT NULL ,
	email                nvarchar(200)  NOT NULL ,
	telefone             nvarchar(11)  NULL ,
	dt_cadastro          datetime  NULL 
)
go

ALTER TABLE Prospect
	ADD CONSTRAINT XPKProspect PRIMARY KEY  CLUSTERED (email ASC)
go

ALTER TABLE Prospect
	ADD CONSTRAINT XUI_Email UNIQUE (email  ASC)
go

CREATE TABLE SMS
( 
	telefone             nvarchar(11)  NOT NULL ,
	smsId                integer IDENTITY ( 1,1 ) ,
	dt_cadastro          datetime  NOT NULL ,
	corretorId           integer  NULL ,
	nome                 nvarchar(30)  NOT NULL 
)
go

ALTER TABLE SMS
	ADD CONSTRAINT XPKSMS PRIMARY KEY  CLUSTERED (smsId ASC)
go


ALTER TABLE Chat
	ADD CONSTRAINT R_2 FOREIGN KEY (corretorId) REFERENCES CorretorOnline(corretorId)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE Chat
	ADD CONSTRAINT R_3 FOREIGN KEY (email) REFERENCES Prospect(email)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE ChatMessage
	ADD CONSTRAINT R_5 FOREIGN KEY (chatId) REFERENCES Chat(chatId)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE ChatMessage
	ADD CONSTRAINT R_7 FOREIGN KEY (corretorId) REFERENCES CorretorOnline(corretorId)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

ALTER TABLE ChatMessage
	ADD CONSTRAINT R_8 FOREIGN KEY (email) REFERENCES Prospect(email)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE CorretorOnline
	ADD CONSTRAINT R_10 FOREIGN KEY (corretoraId) REFERENCES Corretora(corretoraId)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE Mensagem
	ADD CONSTRAINT R_9 FOREIGN KEY (email) REFERENCES Prospect(email)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go


ALTER TABLE SMS
	ADD CONSTRAINT R_6 FOREIGN KEY (corretorId) REFERENCES CorretorOnline(corretorId)
		ON DELETE NO ACTION
		ON UPDATE NO ACTION
go

----------------------
-- Populas as tabelas
----------------------
insert into Parametro(parametroId, nome, valor) values(1, 'emailPinheiroSereni', 'afbleal@gmail.com')
go

insert into Parametro(parametroId, nome, valor) values(2, 'Nome Administração', 'Pinheiro Sereni')
go

insert into Parametro(parametroId, nome, valor) values(3, 'Path Email Anexo', '/APP_DATA/PiazzaToscana.pdf')
go

insert into Parametro(parametroId, nome, valor) values(4, 'E-Mail interno', 'andreborgesleal@live.com')
go

insert into Parametro(parametroId, nome, valor) values(5, 'Nome E-Mail interno', 'Administração Pinheiro Sereni')
go

insert into Parametro(parametroId, nome, valor) values(6, 'Numero de Janelas Ativas no Chat', '5')
go
