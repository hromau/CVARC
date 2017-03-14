export default {
    getMainScore: teamResults => {
        return teamResults.Results.filter(this.isMainScore)[0].Scores;
    },
    
    isMainScore: res => {
        return res.ScoresType.localeCompare("main") || res.ScoresType.toLocaleLowerCase().startsWith("main");
    },

    sumOtherScores: res => {
        return res.Results.filter(r => !this.isMainScore(r)).reduce((sum, cur) => sum + cur.Scores, 0);
    },

    getWinner: match => {
        if (this.getMainScores(match.TeamGameResults[0]) > this.getMainScores(match.TeamGameResults[1])) {
            return match.TeamGameResults[0].Team.Name;
        }
        return match.TeamGameResults[1].Team.Name;
    }
}