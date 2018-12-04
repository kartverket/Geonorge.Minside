class App extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            active: false
        };
    }

    render() {
        return (
            <div className={this.state.active ? 'map' : 'hidden'}>
                <div className="custom-modal">

                </div>
            </div>
        );
    }
}

ReactDOM.render(<App />, document.getElementById('app'));