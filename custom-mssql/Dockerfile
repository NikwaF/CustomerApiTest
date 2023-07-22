FROM mcr.microsoft.com/mssql/server:2019-latest

ENV ACCEPT_EULA=Y
ENV SA_PASSWORD=Test123!

COPY ./TestSera.bak /var/opt/mssql/backup/

RUN (/opt/mssql/bin/sqlservr --accept-eula & ) | /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Test123! -Q "RESTORE DATABASE [TestSera] FROM DISK = '/var/opt/mssql/backup/TestSera.bak' WITH REPLACE;"