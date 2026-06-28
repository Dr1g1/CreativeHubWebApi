git clone https://github.com/Dr1g1/CreativeHub.git
cd CreativeHub
docker compose up -d mongo
docker compose exec mongo mongosh --eval "rs.initiate({_id: 'rs0', members: [{_id: 0, host: 'mongo:27017'}]})"
docker compose up --build