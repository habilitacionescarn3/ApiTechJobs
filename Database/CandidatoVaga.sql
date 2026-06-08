CREATE TABLE CandidatoVaga
(
    Id INT IDENTITY(1,1) PRIMARY KEY,
    IdCandidato INT NOT NULL,
    IdVaga INT NOT NULL,
    Situacao INT NOT NULL,
    FileKey VARCHAR(500) NULL,

    [DataCadastro] DATETIME NULL, 
    [DataAtualizacao] DATETIME NULL, 
    CONSTRAINT FK_CandidatoVaga_Candidato
        FOREIGN KEY (IdCandidato)
        REFERENCES Candidato(Id),

    CONSTRAINT FK_CandidatoVaga_Vaga
        FOREIGN KEY (IdVaga)
        REFERENCES Vaga(Id),

    CONSTRAINT UQ_CandidatoVaga UNIQUE (IdCandidato, IdVaga)
);