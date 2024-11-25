<<< How to Aspire Deployment >>>
https://www.youtube.com/watch?v=7FbiDviUsNU&t=1s

Presteps:
1. Install Another Redis Destop Manager
2. Install Docker: https://medium.com/@praveenr801/introduction-to-redis-cache-using-docker-container-2e4e2969ed3f
Docker run -d -p 6379:6379 --name local-redis  redis
Note: Make sure you can pull image. Normall if you go to setting in dockerhub, you need to confirm email when signed into new machine. 
In the AppHost/Server/UI: must use the same name as "local-redis"


Steps:
1. AppHost

2. Server - ApiService 

3. UI Component - WebUI 

<<< Done >>>