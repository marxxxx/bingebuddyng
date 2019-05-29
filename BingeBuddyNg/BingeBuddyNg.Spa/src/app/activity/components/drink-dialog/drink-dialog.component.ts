import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { DrinkDialogArgs } from './DrinkDialogArgs';
import { DrinkType } from '../../../../models/DrinkType';

@Component({
  selector: 'app-drink-dialog',
  templateUrl: './drink-dialog.component.html',
  styleUrls: ['./drink-dialog.component.css']
})
export class DrinkDialogComponent implements OnInit {

  readonly beerGifs = ['https://media.giphy.com/media/92wsX8GEoNTYA/giphy.gif',
    'https://media.giphy.com/media/osbU9PXXgwHuM/giphy.gif',
    'https://media.giphy.com/media/oDgKc9AGIo372/giphy.gif',
    'https://i.pinimg.com/originals/8f/0d/36/8f0d3672ef0b768fdf8fb999c9ca116e.gif',
    'https://i.imgur.com/KJM8Ma1.gif',
    'https://media.tenor.com/images/8e0becb6e3edc70b8501240959be2861/tenor.gif',
    'https://media1.tenor.com/images/4a4bb1dd4d4390e6f7ff46a483542bff/tenor.gif?itemid=5579804',
    'https://media1.tenor.com/images/f1276e215f0790bbe6b04aba2eb78f16/tenor.gif?itemid=5488034',
    'https://media1.tenor.com/images/fd16ee0cd84b117a9b092604267bf4fc/tenor.gif?itemid=5275300',
    'https://media1.tenor.com/images/97f5f3060f3443230320cdaa813c49ad/tenor.gif?itemid=4782183',
    'https://media.tenor.com/images/5cb28fdc976fb4a6361f739f8587fe1a/tenor.gif',
    'https://media.tenor.com/images/9355518a227edcab65dd3fade726b029/tenor.gif',
    'https://frinkiac.com/gif/S04E16/1090221/1096277.gif',
    'http://gif-finder.com/wp-content/uploads/2017/01/Girl-sculling-beer-unreal.gif',
    'https://i.pinimg.com/originals/8f/d3/2e/8fd32eb9d747fd662c9e30b951b551a9.gif',
    'https://media.giphy.com/media/3o7TKWtIqI2n2Iepwc/giphy.gif',
    'http://www.emugifs.net/wp-content/uploads/2018/05/the-simpsons-duff-beer-contest-2.gif',
    'https://media.giphy.com/media/3vDFxcB9vZNNS/giphy.gif'];
  readonly wineGifs = ['https://media1.tenor.com/images/cf4393066940affd7a6cf7bca24d31c4/tenor.gif',
    'https://media.giphy.com/media/9U7YUWNqG5j56/giphy.gif',
    'https://media.tenor.com/images/9355518a227edcab65dd3fade726b029/tenor.gif',
    // tslint:disable-next-line:max-line-length
    'https://fsmedia.imgix.net/0d/24/c4/4b/93bc/4bb4/a650/9748fc527a23/just-a-little-is-apparently-just-enough-to-get-your-brain-on-the-march-towards-disease-a-new-stud.gif?rect=0%2C0%2C400%2C200&auto=format%2Ccompress&w=400&gifq=35',
    'http://iruntheinternet.com/lulzdump/images/gifs/reaction-girl-starts-clapping-shocked-face-drinking-13903371528.gif',
    'https://thumbs.gfycat.com/ImaginativeSelfassuredClumber-max-1mb.gif',
    'https://i.pinimg.com/originals/ac/64/7e/ac647e6a258d367b11d67ea7c1b77a3c.gif',
    'https://copyhackers.com/wp-content/uploads/2016/11/Game-of-Thrones-Tyrion-Pour-Wine.gif',
    'https://thumbs.gfycat.com/ColdWhoppingBluefish-size_restricted.gif',
    'https://media.giphy.com/media/btZEAKlDWnBcY/giphy.gif',
    'https://www.boredteachers.com/wp-content/uploads/2016/05/female-drinking-wine-gif.gif',
    'http://i.imgur.com/tBciyhE.gif'];
  readonly shotGifs = ['https://media1.tenor.com/images/e1981c3c933343bdd610d1a3b89ad26c/tenor.gif',
    'https://media.tenor.com/images/9355518a227edcab65dd3fade726b029/tenor.gif',
    'https://data.whicdn.com/images/163997713/original.gif',
    'https://media.giphy.com/media/utxF0PQVKNeJG/giphy.gif',
    // tslint:disable-next-line:max-line-length
    'https://i.kinja-img.com/gawker-media/image/upload/s--fDfZiUNU--/c_fill,f_auto,fl_progressive,g_center,h_675,q_80,w_1200/dmymdldpa6r5ddpavnz6.gif',
    'https://media.giphy.com/media/l41lY6L3uv0M9X6Mg/giphy.gif',
    'http://bestanimations.com/Food/Beverages/Alcohol/funny-drinking-alcohol-gif-3.gif',
    'https://66.media.tumblr.com/4e7be87eb5ace716775d77480d0699f9/tumblr_nu6wz3zXU61r2pp2to1_500.gif',
    'https://media.giphy.com/media/3osxYwRanhYxKBohvG/giphy.gif',
    'https://media.giphy.com/media/2ud0W3E7QEV20/giphy.gif',
    'http://i.imgur.com/tBciyhE.gif',
    'http://kriscounette.k.r.pic.centerblog.net/jh82rt70.gif'];
  readonly antiGifs = ['https://media.tenor.com/images/063a21946b6afea28ca76820acecb5e4/tenor.gif',
    'https://media.giphy.com/media/PAujV4AqViWCA/giphy.gif',
    'https://media.tenor.com/images/b7d97514c6a4cc2f47d42f0d6b260445/tenor.gif',
    'https://media.tenor.com/images/3b0f869e7a9266f3fb12bb1eab6a36b6/tenor.gif',
    'https://media.tenor.com/images/e5bdabdd4bdf3325bf52149380d12e29/tenor.gif',
    'https://static1.squarespace.com/static/597386561b631b9a76865d2c/t/5bb656cde79c7095d36e4c9a/1538676451251/tenor.gif'];

  imageUrl: string;

  constructor(@Inject(MAT_DIALOG_DATA) public args: DrinkDialogArgs,
    public dialogRef: MatDialogRef<DrinkDialogComponent>) { }

  ngOnInit() {
    this.imageUrl = this.getRandomDrinkImageUrl();
    setTimeout(() => this.dialogRef.close(), 7000);
  }

  onClose() {
    this.dialogRef.close();
  }

  getRandomDrinkImageUrl(): string {
    let gifs: string[] = null;
    switch (this.args.drinkType) {
      case DrinkType.Beer: {
        gifs = this.beerGifs;
        break;
      }
      case DrinkType.Wine: {
        gifs = this.wineGifs;
        break;
      }
      case DrinkType.Shot: {
        gifs = this.shotGifs;
        break;
      }
      case DrinkType.Anti: {
        gifs = this.antiGifs;
        break;
      }
    }

    return this.getRandomLink(gifs);
  }


  getRandomLink(gifs: string[]): string {
    const index = Math.floor(Math.random() * (gifs.length - 1));
    console.log('random index ' + index);
    return gifs[index];
  }

}
