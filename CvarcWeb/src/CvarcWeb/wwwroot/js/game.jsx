import {Component} from 'react';
import GameHelper from './gameHelper';

class Game extends Component {
    compareResults(res1, res2) {
        return GameHelper.getMainScore(res1) - GameHelper.getMainScore(res2);
    }

    getTeamClasses(gameResults, i) {
        const sortedResults = JSON.parse(JSON.stringify(gameResults)).sort((a, b) => this.compareResults(a, b));
        const isSoloGame = gameResults.length === 1;
        const isWinner = () => this.compareResults(sortedResults[1], gameResults[i]) === 0 &&
                       this.compareResults(sortedResults[0], gameResults[i]) !== 0;
        const isDraw = () => this.compareResults(sortedResults[0], sortedResults[1]) === 0;
        if (isSoloGame) {
            return "team-result draw";
        }
        if (isDraw()) {
            return "team-result pair-draw";
        }
        if (isWinner()) {
            return "team-result winner";
        }
        return "team-result looser";
    }

    render() {
        const game = this.props;
        var teamGameResults = JSON.parse(JSON.stringify(game.TeamGameResults)).sort((a, b) => a.Role.localeCompare(b.Role));
        const results = teamGameResults.map((r, i) => {
            const classes = this.getTeamClasses(teamGameResults, i);
            return <div className={classes} key={r.TeamGameResultId}>
                       <div className="team-name">{r.Team.Name || "Unnamed team"}</div>
                       {r.Results.map(res =>
                           <div className="scores" key={res.ResultId}>
                               <div className="score-type">{res.ScoresType}</div>
                               <div className="score">{res.Scores}</div>
                           </div>)}
                   </div>;
        });

        return (
            <div className={this.props.isHidden ? "hidden-game " : "game"}>
                <div className="game-id">
                    <a>#{game.GameId}</a>
                </div>
                <div className="game-log">
                    <a href={`/Logs/${game.GameId}`}><div></div></a>
                </div>
                <div className="game-results">{results}</div>
            </div>
        );
    }
};

export default Game;