
import com.gitblit.GitBlit
import com.gitblit.Keys
import com.gitblit.models.RepositoryModel
import com.gitblit.models.TeamModel
import com.gitblit.models.UserModel
import com.gitblit.utils.JGitUtils
import java.text.SimpleDateFormat
import org.eclipse.jgit.lib.Repository
import org.eclipse.jgit.lib.Config
import org.eclipse.jgit.revwalk.RevCommit
import org.eclipse.jgit.transport.ReceiveCommand
import org.eclipse.jgit.transport.ReceiveCommand.Result
import org.slf4j.Logger

/**

-->	Register this script in gitblit.properties as 'groovy.postReceiveScripts = dcs-push'

 * Bound Variables:
 *  gitblit			Gitblit Server	 			com.gitblit.GitBlit
 *  repository		Gitblit Repository			com.gitblit.models.RepositoryModel
 *  receivePack		JGit Receive Pack			org.eclipse.jgit.transport.ReceivePack
 *  user			Gitblit User				com.gitblit.models.UserModel
 *  commands		JGit commands 				Collection<org.eclipse.jgit.transport.ReceiveCommand>
 *	url				Base url for Gitblit		String
 *  logger			Logs messages to Gitblit 	org.slf4j.Logger
 *  clientLogger	Logs messages to Git client	com.gitblit.utils.ClientLogger
 *
 * Accessing Gitblit Custom Fields:
 *   def myCustomField = repository.customFields.myCustomField
 *  
 */


class DcsClient {
    def logger

    def sendCommit(repoName, userName, commit) {

        SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss.SSSZ", Locale.US);
        dateFormat.setTimeZone(TimeZone.getTimeZone("GMT"));

        def process = new ProcessBuilder("c:/dcs/bin/dcs.console/dcs.console.exe", 
	        "publish", "userCommitPush", 
            "/commitId", commit.id.name, 
	        "/repository", repoName, 
	        "/userName", userName, 
	        "/committedAt", dateFormat.format(JGitUtils.getCommitDate(commit)),
	        "/message", commit.shortMessage
            )
        .start()

        try {
		    def outReader = Thread.start {
			    process.inputStream.eachLine {line -> logger.info(line)}
		    }
		    def errReader = Thread.start {
			    process.errorStream.eachLine {line -> logger.info(line)}
		    }
		    process.waitFor()
		    outReader.join()
		    errReader.join()
	    } finally {
	        process.destroy()
	    }

        if (process.exitValue() != 0) {
	        logger.error("Failed to call DCS post-receive hook")
        }
        else {
	        logger.info("DCS commit sent")
        }
    }
}

def dcsClient = new DcsClient();
dcsClient.logger = logger;
 
logger.info("DCS hook triggered by ${user.username} for ${repository.name}")

Repository repo = gitblit.getRepository(repository.name)

def commitCount = 0
for (command in commands) {
    logger.info("command: ${command.type} ${command.refName}")
    switch (command.type) {
	    case ReceiveCommand.Type.CREATE:
        case ReceiveCommand.Type.UPDATE:
	    case ReceiveCommand.Type.UPDATE_NONFASTFORWARD:
		    def commits = JGitUtils.getRevLog(repo, command.oldId.name, command.newId.name).reverse()
		    commitCount += commits.size()
            for(commit in commits) {
                logger.info("[${JGitUtils.getCommitDate(commit)}] ${JGitUtils.getDisplayName(commit.authorIdent)} ${commit.id.name} ${commit.shortMessage}")
                dcsClient.sendCommit(repository.name, user.username, commit)
            }
		    break
	    case ReceiveCommand.Type.DELETE:
	    default:
		    break
    }
}

