import {Component} from 'react';
import Game from './game';
import {getMainScore, isMainScore, sumOtherScores, getWinner} from './gameHelper';

class OlympicTournament extends Component {
    getStagesFromTree(tournamentTree) {
        const stages = [];
        const queue = [{ match: tournamentTree.FinalMatch, level: 0 }];
        while (queue.length !== 0) {
            const current = queue.shift();
            if (!stages[current.level]) {
                stages[current.level] = [];
            }
            stages[current.level].push(current.match);
            if (!current.match.FirstPreviousStageMatch) {
                continue;
            }
            queue.push({ match: current.match.FirstPreviousStageMatch, level: current.level + 1 });
            queue.push({ match: current.match.SecondPreviousStageMatch, level: current.level + 1 });
        }
        return stages.reverse();
    }
    
    getGamesFromStage(stage, i) {
        const games = stage.map(match => {
            const results = match.Game.TeamGameResults;
            const topWinner = this.getMainScores(results[1]) > this.getMainScores(results[0]);
            return [
                <li className={`game game-top${topWinner ? " winner" : ""}`} key={match.Game.GameId + (topWinner ? " winner" : "")}>
                    <span className="team-name">{results[1].Team.Name}</span>
                    <span className="score">{this.getMainScores(results[1])}</span>
                </li>,
                <li className="game game-spacer" key={match.Game.GameId + " game-spacer"}>&nbsp;<Game {...match.Game} isHidden={true} stage={i} key={match.Game.GameId + " hidden-game"} /></li>,
                <li className={`game game-bottom${!topWinner ? " winner" : ""}`} key={match.Game.GameId + (!topWinner ? " winner" : "")}>
                    <span className="team-name">{results[0].Team.Name}</span>
                    <span className="score">{this.getMainScores(results[0])}</span>
                </li>,
                <li className="spacer" key={match.Game.GameId + " spacer"}>&nbsp;</li>
            ];
        });
        const result = [];
        for (var i = 0; i < games.length; i++) {
            result.push(
                games[i][0],
                games[i][1],
                games[i][2],
                games[i][3]
            );
        }
        return result;
    }

    render() {
        const tournamentTree = this.props.tournament;
        const thirdPlaceGame = this.props.tournament.ThirdPlaceMatch.Game;
        const topWinner = getMainScores(thirdPlaceGame.TeamGameResults[0]) > (thirdPlaceGame.TeamGameResults[1]);
        const stages = this.getStagesFromTree(tournamentTree);
        const winnerTeam = getWinner(tournamentTree.FinalMatch.Game);
        const thirdTeam = getWinner(tournamentTree.ThirdPlaceMatch.Game);

        return (
        <div>
            <div>
                <div className="tournament">
                {
                    stages.map((stage, i) => 
                        <ul className={`round round-${i + 1}`} key={i}>
        		            <li className="spacer">&nbsp;</li>
                            {this.getGamesFromStage(stage, i)}
                        </ul>)
                    
                }
                    <ul className={`round round-${stages.length + 1}`}>
                        <li className="game game-top winner">
                            <span className="team-name">{winnerTeam}</span>
                        </li>
                    </ul>
                </div>
            </div>
            <div className="tournament">
                {stages.map((_, i) => <ul className="round round-1" key={i}></ul>).slice(0, stages.length - 1)}
                <ul className="round round-1">
        		    <li className="spacer">&nbsp;</li>
        		    <li className="spacer">&nbsp;</li>
                    <li className={`game game-top${topWinner ? " winner" : ""}`} key={thirdPlaceGame.GameId + (topWinner ? " winner" : "")}>
                        <span className="team-name">{thirdPlaceGame.TeamGameResults[0].Team.Name}</span>
                        <span className="score">{getMainScores(thirdPlaceGame.TeamGameResults[0])}</span>
                    </li>
                    <li className="game game-spacer" key={thirdPlaceGame.GameId + " game-spacer"}>&nbsp;<Game {...thirdPlaceGame} isHidden={true} stage={0} key={thirdPlaceGame.GameId + " hidden-game"} /></li>
                    <li className={`game game-bottom${!topWinner ? " winner" : ""}`} key={thirdPlaceGame.GameId + (!topWinner ? " winner" : "")}>
                        <span className="team-name">{thirdPlaceGame.TeamGameResults[1].Team.Name}</span>
                        <span className="score">{getMainScores(thirdPlaceGame.TeamGameResults[1])}</span>
                    </li>
                    <li className="spacer" key={thirdPlaceGame.GameId + " spacer"}>&nbsp;</li>
                </ul>
                <ul className="round round-2">
                    <li className="game game-top winner">{thirdTeam}</li>
                </ul>
            </div>
        </div>
        );
    }
}

export default OlympicTournament;