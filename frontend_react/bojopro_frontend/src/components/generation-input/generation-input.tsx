import React, { useEffect } from 'react';
import tabMonthsEnglish from '../../ressources/tab-months';
import './generation-input.css';
import { Node } from 'typescript';



function GenerationInput()
{
    const handlePdf = () => {
        console.log('testReussi')
        fetch('http://localhost:80/BujoPro/GetBuJoPDFV1', {
            }).then(response => {
            if(!response.ok)
            {
                throw new Error("Error fetching the pdf");
            }
            return response.blob()
          }).then((blob) => {
                const blobUrl = URL.createObjectURL(blob);
                const lienPdf : HTMLElement | null = document.getElementById("lienPdf");
                if(lienPdf === null)
                {
                    throw new Error("The link to the pdf was not found")
                }

                lienPdf.setAttribute("href",  blobUrl);;
                lienPdf.setAttribute("download", "Custom-Boju");
                lienPdf.innerText = "Click here to download your agenda"
          }
        ).catch(
            (error) => {
                console.log(error)
            }
        );
    }




    return <div>
            <form>
                <label className="month_input" htmlFor="start_month">First month</label>
                <select className="month_input" id="start_month">
                { 
                    tabMonthsEnglish.map
                    (
                        month =>
                        (
                            <option key={month.id} value={month.id}>{month.name}</option>
                        )
                    )   
                }
                </select>
                <label className="month_input" htmlFor="number_months">Number of months</label>
                <input className="month_input" id="number_months" type="number" min="1" max="12"></input>
                <a  onClick={() => handlePdf()}>Create calendar</a>
                <a id="lienPdf"></a>
            </form>
        </div>    
}

export default GenerationInput;