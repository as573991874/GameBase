rm -rf $1
mkdir $1
mkdir $1/Payload
cp -r $1.app $1/Payload/$1.app
cp Icon.png $1/iTunesArtwork
cd $1
zip -r $1.ipa Payload iTunesArtwork
exit 0