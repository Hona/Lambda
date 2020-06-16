echo -e '-= Stopping lambda Production Container =-\n'
docker container stop lambda-production

echo -e '-= Removing Old lambda Production Container =-\n'
docker container rm lambda-production

echo -e '-= Building Docker Image from Dockerfile =-\n'
docker build -t lambda -f src/

echo -e '-= Running the Image =-\n'
docker run -v $PWD/config:/app/config --network host --restart on-failure:5 --name "lambda-production" -d lambda
