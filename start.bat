set MACHINE_IP=10.0.75.1
docker-compose build
START "consensus" docker-compose up
TIMEOUT 8
start "" http://localhost:5000/swagger