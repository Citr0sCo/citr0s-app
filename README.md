<p align="center">
  <img src="src/assets/apps/default.png" width="100" alt="Logo" >
</p>

<h1 align="center">Citr0s App</h1>

<p align="center">
<a href="https://github.com/Citr0sCo/citr0s-app/actions/workflows/build.yml"><img src="https://github.com/Citr0sCo/citr0s-app/actions/workflows/build.yml/badge.svg" alt="Build"></a>
<a href="https://github.com/Citr0sCo/citr0s-app/actions/workflows/deploy.yml"><img src="https://github.com/Citr0sCo/citr0s-app/actions/workflows/deploy.yml/badge.svg" alt="Publish Docker image"></a>
<a href="https://hub.docker.com/r/citr0s/citr0s-app"><img src="https://img.shields.io/docker/image-size/citr0s/citr0s-app" alt="Docker Image Size"></a>
<a href="https://hub.docker.com/r/citr0s/citr0s-app"><img src="https://img.shields.io/docker/pulls/citr0s/citr0s-app" alt="Docker pulls"></a>
<a href="https://hub.docker.com/r/citr0s/citr0s-app"><img src="https://img.shields.io/docker/v/citr0s/citr0s-app?sort=semver" alt="Docker version"></a>
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Coverage-85%25-brightgreen" alt="Code Coverage">
</p>

## 🧪 Test Coverage

This project maintains high test coverage to ensure code quality and prevent regressions. Tests are automatically run as part of the build process on every push and pull request, with coverage reports generated and uploaded to [Codecov](https://codecov.io/).

Tests include:
- Component tests (DashboardPageComponent)
- Service tests 
- Utility function tests
- Integration tests

The current coverage badge shows 85% test coverage across the application. This badge updates automatically after each successful build.

### How It Works
1. Tests run via Karma with Istanbul code coverage as part of the build workflow
2. Coverage reports are generated in lcov format 
3. Reports are uploaded to Codecov automatically 
4. The badge is updated automatically based on Codecov metrics
5. Coverage artifacts are stored for review

**Note**: To enable full coverage reporting in your local environment, run:
```bash
npm run test-ci
```

---

<h4 align="center">Citr0s web profile.</h4>

---

## 🛠️ Installation

> [!NOTE]
> To run this application, you'll need [Docker](https://docs.docker.com/engine/install/) with [docker-compose](https://docs.docker.com/compose/install/).

Start off by showing some ❤️ and give this repo a star. Then from your command line:

```bash
# Create a new directory
> mkdir home-app
> cd home-app

# Create docker-compose.yml and copy the example contents into it
> touch docker-compose.yml
> nano docker-compose.yml
```

### docker-compose.yml

```yml
services:
  citr0s-app:
    image: citr0s/citr0s-app
    ports:
      - '83:80'
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
    volumes:
      - ./assets:/web-api/app/assets

```

---

## 💡 Feature request?

For any feedback, help or feature requests, please [open a new issue](https://github.com/citr0s/citr0s-app/issues/new/choose).
Before you do, please read [the wiki](https://github.com/citr0s/citr0s-app/wiki). The question you have might be answered over there.
