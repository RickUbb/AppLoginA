SELECT * FROM Usuarios;
SELECT * FROM Roles;

INSERT INTO Usuarios (Correo, Password, idRol)
VALUES ('rpvallet@uce.edu.ec', '9a900403ac313ba27a1bc81f0932652b8020dac92c234d98fa0b06bf0040ecfd', 1);

INSERT INTO Logs (Correo, IsRegisteredToken) 
VALUES ('rpvallet@uce.edu.ec', 1);


