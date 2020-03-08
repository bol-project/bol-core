set MACHINE_IP=10.0.75.1
docker-machine start default
docker-compose build
START "consensus" docker-compose up
TIMEOUT 8
start "" http://192.168.99.100:5000/swagger