import {Component} from 'react';

class Game extends Component {
    compareResults(res1, res2) {
        if (this.getMainScore(res1) !==  this.getMainScore(res2)) {
            return this.getMainScore(res1) - this.getMainScore(res2);
        }
        return this.sumOtherScores(res1) - this.sumOtherScores(res2);
    }
    
    isMainScore(res) {
        return res.ScoresType === "MainScores";
    }

    sumOtherScores(res) {
        return res.Results.filter(r => !this.isMainScore(r)).reduce((sum, cur) => sum + cur.Scores, 0);
    }

    getMainScore(teamResults) {
        return teamResults.Results.filter(this.isMainScore)[0].Scores;
    }

    getTeamClasses(game, i) {
        const sortedResults = JSON.parse(JSON.stringify(game.TeamGameResults)).sort((a, b) => this.compareResults(a, b));
        const isPvp = game.TeamGameResults.length > 1;
        const isWinner = () => this.compareResults(sortedResults[1], game.TeamGameResults[i]) === 0 &&
                       this.compareResults(sortedResults[0], game.TeamGameResults[i]) !== 0;
        const isDraw = () => this.compareResults(sortedResults[0], sortedResults[1]) === 0;
        return isPvp && !isDraw()
                    ? isWinner() 
                        ? "team-result winner" 
                        : "team-result looser" 
                    : "team-result";

    }

    render() {
        const game = this.props;
        const results = game.TeamGameResults.map((r, i) => {
            const classes = this.getTeamClasses(game, i);
            return <div className={classes} key={r.TeamGameResultId}>
                       <div className="team-name">{r.Team.Name}</div>
                       {r.Results.map(res =>
                           <div className="scores" key={res.ResultId}>
                               <div className="score-type">{res.ScoresType}</div>
                               <div className="score">{res.Scores}</div>
                           </div>)}
                   </div>;
        });

        return (
            <div className={this.props.isHidden ? "hidden-game " : "game"} data-stage={this.props.stage + 1}>
                <div className="game-id">
                    <a href={game.PathToLog}>#{game.GameId}</a>
                </div>
                <div className="game-log">
                    <a href={game.PathToLog}><div></div></a>
                </div>
                <div className="game-results">{results}</div>
            </div>
        );
    }
};

export default Game;