#!/bin/bash

# Exit immediately if a command exits with a non-zero status
set -e
# Treat unset variables as an error when substituting.
set -u

if [ "$1" = "/opt/mssql/bin/sqlservr" ]; then
  # If this is the container's first run, initialize the application database
  if [ ! -f /tmp/app-initialized ]; then
    # Initialize the application database asynchronously in a background process.
    # This allows
    ## a) the SQL Server process to be the main process in the container,
    ##    which allows graceful shutdown and other goodies, and
    ## b) us to only start the SQL Server process once, as opposed to starting,
    ##    stopping, then starting it again.
    function initialize_app_database() {
      while : ; do
          if /opt/mssql-tools18/bin/sqlcmd -C -S localhost -U sa -P ${MSSQL_SA_PASSWORD} -Q "SELECT 1" -b
          then
              echo "[entrypoint.sh] Connected, starting creating tables."
              break;
          fi

          echo "[entrypoint.sh] Cannot connect.. retrying in 5s"
          sleep 5
      done

      # Note that the container has been initialized so future starts won't wipe changes to the data
      touch /tmp/app-initialized
    }
    initialize_app_database &
  fi
fi

exec "$@"
