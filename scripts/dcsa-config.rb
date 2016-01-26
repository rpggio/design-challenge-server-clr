
def appConfigLocal
    {
        name: 'local',
        transport: "http",
        userReposDir: "c:/dcs-test/user-repos",
        adminReposDir: "c:/dcs-test/admin-repos",
        expandDir: "c:/dcs-test/expand",
        tempDir: "c:/dcs-test/temp",
        gitblitRpcUrl: "https://localhost:8443",
        gitblitServer: "localhost:8080",
        dcsApiUrl: "http://localhost:8281",
        adminUsername: "admin",
        adminPassword: "admin",
        username: "testUser%{suffix}",
        password: "password%{suffix}",
        userEmail: "dcs-%{username}@mailinator.com",
        repoName: "testRepo%{suffix}",
        repoPath: "%{repoName}.git",
        stagesDir: "C:/work/me/design-challenge-server/challenges/GateScheduler/stages",
        mailServerApiUrl: "http://127.0.0.1:1080",
        referenceSolution: "cs-nancy"

        #repoUrl: "https://%{username}:%{password}@%{gitblitServer}/r/%{repoPath}" 
    }
end


