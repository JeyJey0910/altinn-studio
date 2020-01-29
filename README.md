# Altinn Studio

[![Altinn Studio build status](https://dev.azure.com/brreg/altinn-studio/_apis/build/status/altinn-studio-build-designer-image-v2-master?label=Altinn%20Studio)](https://dev.azure.com/brreg/altinn-studio/_build/latest?definitionId=18)
[![Altinn Apps build status](https://dev.azure.com/brreg/altinn-studio/_apis/build/status/altinn-studio-build-runtime-image?label=Altinn%20Apps)](https://dev.azure.com/brreg/altinn-studio/_build/latest?definitionId=6)

Altinn Studio is the next generation Altinn application development solution. Together with Altinn Apps and Altinn Platform, it makes a complete application development and hosting platform.

An early test version of Altinn Studio is available at https://altinn.studio.

Use the [documentation](https://docs.altinn.studio/) to get started using Altinn Studio.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes. See deployment for notes on how to deploy the project on a live system.


### Installing

Clone [Altinn Studio repo](https://github.com/Altinn/altinn-studio) and navigate to the folder.

```cmd
git clone https://github.com/Altinn/altinn-studio
cd altinn-studio
```

#### Altinn Studio

Follow the instructions found [here](/src/studio/).


#### Apps
> If you only need to develop and debug App-Frontend, you can follow the description in 5) (only) and deploy the app to any test environment. The App-Frontend will be loaded from your local webpack-dev-server.

It's possible to run an app locally in order to test and debug it. It needs a local version of the platform services to work.

_NOTE: Currently, it is not possible to run Apps and Altinn Studio (designer) in parallel. To run Apps, make sure that none of the containers for Altinn Studio are running, f.ex. by navigating to the root of the altinn-studio repo, and running the command

```cmd
docker-compose down
```

**Setting up local platform services for test**

1. Navigate to the `development` folder in the altinn-studio repo

```cmd
cd src/development
```

2. Start the loadbalancer container that routes between the local platform services and the app

```cmd
docker-compose up -d --build
```

3. Set path to app folder in local platform services:
 - Open `appSettings.json` in the `LocalTest` folder, f.ex. in Visual Studio Code

```cmd
cd LocalTest
code appSettings.json
```

 - Change the setting `"AppRepsitoryBasePath"` to the full path to your app on the disk. Save the file.

4. Start the local platform services (make sure you are in the LocalTest folder)

```cmd
dotnet run
```

5. Navigate to the app folder (specified in the step above)

```cmd
cd \.\<path to app on disk>
```

- If you need to debug (or run locally) the app front-end:
  - Open the file `views/Home/Index.cshtml` and change the line

  ```html
   <script src="https://altinncdn.no/toolkits/altinn-app-frontend/1/altinn-app-frontend.js"></script>
    ```

    to

    ```html
     <script src="http://localhost:8080/altinn-app-frontend.js"></script>
    ```

    - Build and run the runtime front-end project locally (`altinn-studio/src/Altinn.Apps/AppFrontend/react`):
    ```cmd
    npm install # only needed first time, or when dependencies are updated
    npm run install-deps # only needed first time, or when dependencies are updated
    cd altinn-app-frontend
    npm start
    ```

6. Start the app locally

```cmd
dotnet run -p App.csproj
```

The app and local platform services are now running locally. The app can be accessed on altinn3local.no.

Log in with a test user, using your app name and org name. This will redirect you to the app.

#### Building other react apps
If you need to rebuild other react apps, for instance Dashboard or ServiceDevelopment, this can be done by navigating to their respective folders, example `src/react-apps/applications/dashboard` and then run the following build script

```cmd
npm run build
```
Some of the react projects also have various other predefined npm tasks, which can be viewed in the `package.json` file which is located in the root folder of each react project, example `src/react-apps/applications/dashboard/package.json`

#### Platform Receipt
The platform receipt component can run locally, both in docker and manually.

**Manual**
- Open a terminal in `src/Altinn.Platform/Altinn.Platform.Receipt`
- run `npm install`
- run `npm run gulp` (if running for the first time, otherwise this can be skipped)
- run `npm run gulp-install-deps`
- run `npm run gulp-develop`

This will build and run receipt back end, and build and copy the receipt frontend to the `wwwroot` folder.
The application should now be available at `localhost:5060/receipt/{instanceOwnerId}/{instanceId}`
The script wil also listen to changes in the receipt react app, rebuild and copy the new react app to the `wwwroot` folder.

**Docker**
- Open a terminal in `src/Altinn.Platform/Altinn.Platform.Receipt`
- run `docker-compose up`
- The application should now be available at `localhost:5060/receipt/{instanceOwnerId}/{instanceId}`

## Running the tests

### End to end tests

Automated end to end tests are currently being developed.

### Coding style tests

Coding style tests are available for the React front end application, using _tslint_.

Navigate to the React front end application and run linting.

```cmd
cd src/react-apps/applications/ux-editor
npm run lint
```

## Deployment

The current build is deployed in Kubernetes on Azure.

Automated build/deploy process is being developed.

## Built With

- [React](https://reactjs.org/)/[Redux](https://redux.js.org/) - The front-end framework
- [.NET Core](https://docs.microsoft.com/en-us/dotnet/core/)/[C#](https://docs.microsoft.com/en-us/dotnet/csharp/) - The back-end framework
- [npm](https://www.npmjs.com/) - Package management
- [Docker](https://www.docker.com/) - Container platform
- [Kubernetes](https://kubernetes.io/) - Container orchestration

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md) for details on our code of conduct, and the process for submitting pull requests to us.

## Authors

- **Altinn Studio development team** - If you want to get in touch, just [create a new issue](https://github.com/Altinn/altinn-studio/issues/new).

See also the list of [contributors](https://github.com/Altinn/altinn-studio/graphs/contributors) who participated in this project.

## License

This project is licensed under the 3-Clause BSD License - see the [LICENSE.md](LICENSE.md) file for details.
