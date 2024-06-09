docker build . -f ./MSA.ProductService/Dockerfile -t product-service:latest
docker build . -f ./MSA.IdentityServer/Dockerfile -t identity-service:latest
docker build . -f ./MSA.ReverseProxy/Dockerfile -t reverse-proxy:latest