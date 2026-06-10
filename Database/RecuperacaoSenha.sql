CREATE TABLE RecuperacaoSenha
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario INT NOT NULL,
    TokenHash VARCHAR(200) NOT NULL,
    DataExpiracao DATETIME NOT NULL,
    DataCriacao DATETIME NOT NULL,
    DataUtilizacao DATETIME NULL,

    CONSTRAINT FK_RecuperacaoSenha_Usuario
        FOREIGN KEY (IdUsuario)
        REFERENCES Usuario(Id)
);
