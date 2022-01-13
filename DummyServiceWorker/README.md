## DummyServiceWorker

This project is an example of how to use TinyHealthCheck. It can be started either from VisualStudio, or by
build/running the Dockerfile. Ports 8080, 8081, and 8082 will each have respective health-checks. This
container demonstrates how using `*` as the hostname within a Linux container is permitted.

### Build Container

From the root of this repo, run:

```
docker build -t dummy-service-worker -f ./DummyServiceWorker/Dockerfile .
docker run -it -p 8080:8080 -p 8081:8081 -p 8082:8082 dummy-service-worker
```

Once running, pointing your browser to `http://localhost:[8080|8081|8082]` will produce the expected health-check output.

**On native Windows, you must run the process as an administrator to use * as the hostname! Failure to do this will result in the TinyHealthCheck process failing. This is not required if running from the Dockerfile**
