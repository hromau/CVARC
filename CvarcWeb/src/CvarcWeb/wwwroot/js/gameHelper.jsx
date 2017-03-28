const getMainScore = teamResults => teamResults.Results.filter(isMainScore)[0].Scores;
const isMainScore = res => res.ScoresType.localeCompare("main") === 0 || res.ScoresType.toLocaleLowerCase().startsWith("main");
const sumOtherScores = res => res.Results.filter(r => !isMainScore(r)).reduce((sum, cur) => sum + cur.Scores, 0);
const getWinner = match => {
    if (getMainScore(match.TeamGameResults[0]) > getMainScore(match.TeamGameResults[1])) {
        return match.TeamGameResults[0].Team.Name;
    }
    return match.TeamGameResults[1].Team.Name;
}

export default {
    getMainScore,
    isMainScore,
    sumOtherScores,
    getWinner
}