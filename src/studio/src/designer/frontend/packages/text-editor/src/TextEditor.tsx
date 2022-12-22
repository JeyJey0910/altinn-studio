import React, { useCallback, useEffect, useState } from 'react';
import classes from './TextEditor.module.css';
import type {
  LangCode,
  TextResourceEntry,
  TextResourceFile,
  TextResourceEntryDeletion,
  TextResourceIdMutation,
} from './types';
import { AltinnSpinner } from 'app-shared/components';
import { Button, ButtonColor, ButtonVariant } from '@altinn/altinn-design-system';
import { RightMenu } from './RightMenu';
import { getRandNumber } from './utils';
import {
  generateTextResourceFile,
  mapTextResources,
  removeTextEntry,
  updateTextEntryId,
  upsertTextEntry,
} from './mutations';
import { defaultLangCode } from './constants';
import { TextList } from './TextList';

export interface TextEditorProps {
  translations: TextResourceFile;
  isFetchingTranslations: boolean;
  onTranslationChange: (translations: TextResourceFile) => void;
  selectedLangCode: string;
  setSelectedLangCode: (langCode: LangCode) => void;
  availableLangCodes: string[];
  onAddLang: (langCode: LangCode) => void;
  onDeleteLang: (langCode: LangCode) => void;
}

export const TextEditor = ({
  translations,
  selectedLangCode,
  onTranslationChange,
  isFetchingTranslations,
  setSelectedLangCode,
  availableLangCodes,
  onAddLang,
  onDeleteLang,
}: TextEditorProps) => {
  const handleSelectedLangChange = (langCode: LangCode) => setSelectedLangCode(langCode);
  const { resources } = translations;
  const [textIds, setTextIds] = useState(resources.map(({ id }) => id) || []);
  const getUpdatedTexts = useCallback(() => mapTextResources(resources), [resources]);
  const [texts, setTexts] = useState(getUpdatedTexts());
  useEffect(() => {
    if (!selectedLangCode) {
      setSelectedLangCode(defaultLangCode);
    }
  }, [selectedLangCode, setSelectedLangCode]);
  useEffect(() => {
    setTexts(getUpdatedTexts());
  }, [getUpdatedTexts, resources]);

  const handleAddNewEntryClick = () => {
    const newId = `id_${getRandNumber()}`;
    onTranslationChange(
      upsertTextEntry(translations, {
        id: newId,
        value: '',
      })
    );
    setTextIds([newId, ...textIds]);
  };
  const removeEntry = ({ textId }: TextResourceEntryDeletion) => {
    const mutatedIds = textIds.filter((v) => v !== textId);
    const mutatedEntries = removeTextEntry(texts, textId);
    onTranslationChange(
      generateTextResourceFile(translations.language, mutatedIds, mutatedEntries)
    );
    setTextIds(mutatedIds);
  };
  const upsertEntry = (entry: TextResourceEntry) =>
    onTranslationChange(upsertTextEntry(translations, entry));
  const updateEntryId = ({ oldId, newId }: TextResourceIdMutation) => {
    onTranslationChange(updateTextEntryId(translations, oldId, newId));
    const mutatingIds = [...textIds];
    const idx = mutatingIds.findIndex((v) => v === oldId);
    mutatingIds[idx] = newId;
    setTextIds(mutatingIds);
  };

  return (
    <div className={classes.TextEditor}>
      <div className={classes.TextEditor__main}>
        <div className={classes.TextEditor__topRow}>
          <Button
            variant={ButtonVariant.Filled}
            color={ButtonColor.Primary}
            onClick={handleAddNewEntryClick}
            disabled={isFetchingTranslations}
          >
            Ny tekst
          </Button>
        </div>
        <TextList
          textIds={textIds}
          selectedLangCode={selectedLangCode}
          texts={texts}
          upsertEntry={upsertEntry}
          removeEntry={removeEntry}
          updateEntryId={updateEntryId}
        />
        {isFetchingTranslations ? (
          <div>
            <AltinnSpinner />
          </div>
        ) : null}
      </div>
      <RightMenu
        onAddLang={onAddLang}
        onDeleteLang={onDeleteLang}
        selectedLangCode={selectedLangCode}
        onSelectedLangChange={handleSelectedLangChange}
        availableLangCodes={availableLangCodes || [defaultLangCode]}
      />
    </div>
  );
};
