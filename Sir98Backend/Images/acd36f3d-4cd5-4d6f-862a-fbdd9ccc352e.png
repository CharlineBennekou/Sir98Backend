import * as React from 'react';



export class CodeActivationComp extends React.Component {
    private success: boolean = true;
    private loading: boolean = true;

    constructor(props: {}) {
        super(props);
        const parameters = new URLSearchParams(location.search)
        const code: string | null = parameters.get("code");
        console.log(code)
        if(!code) {
            this.success = false;
            alert(code);
            this.forceUpdate();
        }

        fetch(`https://localhost:7275/api/User/Activate/code=${code}`, {
            method: 'GET',
        }).then((response) => {
            if(response.ok) {
                this.success = true;
            } else {
                this.success = false;
            }
            console.log(response)
            this.forceUpdate();
        });
    }

    public render(): React.ReactNode {
        
        if(this.loading) {
            return (<h1>Aktiverer konto</h1>);
        }
        if(this.success) {
            return (<h1>Konto er nu aktiveret</h1>);
        }
        return (
            <h1>Ugyldig aktiveringskode.</h1>
        );
    }
}

