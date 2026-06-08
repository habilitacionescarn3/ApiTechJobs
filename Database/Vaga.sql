CREATE TABLE Vaga
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IdEmpresa INT NOT NULL,
    Nome VARCHAR(200) NOT NULL,
    Cargo VARCHAR(150) NOT NULL,
    Modelo VARCHAR(100) NOT NULL,
    NivelExperiencia VARCHAR(100) NOT NULL,
    Cep CHAR(8) NULL,
    Numero VARCHAR(20) NULL,
    Descricao VARCHAR(MAX) NOT NULL,
    SalarioPrevisto DECIMAL(18,2) NULL,
    Interna BIT NOT NULL DEFAULT 0,
    DataCadastro DATETIME NOT NULL DEFAULT GETDATE(),
    DataFimInscricoes DATETIME NOT NULL,

    [Tecnologias] VARCHAR(2000) NULL, 
    [Requisitos] VARCHAR(2000) NULL, 
    [Beneficios] VARCHAR(2000) NULL, 
    CONSTRAINT FK_Vaga_Empresa
        FOREIGN KEY (IdEmpresa)
        REFERENCES Empresa(Id)
);